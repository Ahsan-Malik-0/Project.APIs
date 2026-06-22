using Microsoft.EntityFrameworkCore;
using Project.APIs.Database;
using Project.APIs.Exceptions;
using Project.APIs.Model;
using Project.APIs.Model.DTOs;
using System.Net.WebSockets;

namespace Project.APIs.Services
{
    public class EventService(DB _dB)
    {

        //Get all pending events
        public async Task<List<Event>> GetPendingEvents(Guid memberId)
        {
            // Fix: Get the SocietyId for the given memberId
            var pendingEvents = await _dB.Events
                                .Include(e => e.Requirements)
                                .Where(e => e.Status == "pending")
                                .Where(e => _dB.Members
                                    .Any(m => m.Id == memberId && m.SocietyId == e.SocietyId))
                                .ToListAsync();

            if (!pendingEvents.Any())
                throw new NotFoundException("Pending events not found");

            return pendingEvents;
        }

        public async Task<List<Event>> GetPendingAndRejectedEvents(Guid memberId)
        {
            // Fix: Get the SocietyId for the given memberId
            var pendingEvents = await _dB.Events
                                .Include(e => e.Requirements)
                                .Where(e => e.Status == "pending" || e.Status == "rejected")
                                .Where(e => _dB.Members
                                    .Any(m => m.Id == memberId && m.SocietyId == e.SocietyId))
                                .ToListAsync();

            if (!pendingEvents.Any())
                throw new NotFoundException("Pending events not found");

            return pendingEvents;
        }

        //Add a new event from President
        public async Task AddEvent(AddEventDto newEvent, string status)
        {
            using var transaction = await _dB.Database.BeginTransactionAsync();
            try
            {
                // Check if conflicting event exists
                // Step 1: get non-financial requirement names from new event
                var newNonFinancialReqNames = newEvent.Requirements
                    .Where(r => r.Type == "non-financial")
                    .Select(r => r.Name)
                    .ToList();

                // Step 2: check overlapping events + matching requirements
                var conflictingEvent = await _dB.Events
                    .Include(e => e.Requirements)
                    .FirstOrDefaultAsync(e =>
                        e.Status == "pending" &&
                        e.EventDate.Date == newEvent.EventDate.Date &&

                        // exclude self (important if updating)
                        //e.Id != newEvent.Id &&

                        // TIME OVERLAP CHECK
                        newEvent.StartTime < e.EndTime &&
                        newEvent.EndTime > e.StartTime &&

                        // REQUIREMENT CONFLICT CHECK
                        e.Requirements.Any(r =>
                            r.Type == "non-financial" &&
                            newNonFinancialReqNames.Contains(r.Name)
                        )
                    );

                // Step 3: throw exception if conflict found
                if (conflictingEvent != null)
                {
                    var conflictingNames = conflictingEvent.Requirements
                        .Where(r =>
                            r.Type == "non-financial" &&
                            newNonFinancialReqNames.Contains(r.Name))
                        .Select(r => r.Name)
                        .Distinct();

                    throw new BusinessRuleException(
                        $"Non-financial requirements '{string.Join(", ", conflictingNames)}' " +
                        $"already exist in event '{conflictingEvent.Name}'"
                    );
                }

                var _event = new Event
                {
                    Name = newEvent.Name,
                    StartTime = newEvent.StartTime,
                    EndTime = newEvent.EndTime,
                    EventDate = newEvent.EventDate,
                    Status = status,
                    ReviewMessage = null,
                    SocietyId = newEvent.SocietyId,
                    VirtualSocietyId = newEvent.VirtualSocietyId,
                    Requirements = null!
                };

                // Add event first
                await _dB.Events.AddAsync(_event);
                await _dB.SaveChangesAsync(); // generates _event.Id

                //Add requirements(if any)
                if (newEvent.Requirements != null && newEvent.Requirements.Any())
                {
                    foreach (var requirement in newEvent.Requirements)
                    {
                        var eventRequirement = new EventRequirement
                        {
                            Type = requirement.Type,
                            Name = requirement.Name,
                            Price = requirement.Price,
                            Quantity = requirement.Quantity,
                            EventId = _event.Id
                        };

                        await _dB.EventRequirements.AddAsync(eventRequirement);
                    }
                }


                await _dB.SaveChangesAsync();


                await transaction.CommitAsync();
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                throw new BusinessRuleException("Unable to save event. Please try again.");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw; // goes to 500 handler
            }
        }


        // Update Event with Requirements
        public async Task UpdateEventWithRequirements(Guid id, UpdateEventDto dto)
        {
            using var transaction = await _dB.Database.BeginTransactionAsync();

            // Step 1: get non-financial requirement names from new event
            var newNonFinancialReqNames = dto.Requirements
                .Where(r => r.Type == "non-financial")
                .Select(r => r.Name)
                .ToList();

            // Step 2: check overlapping events + matching requirements
            var conflictingEvent = await _dB.Events
                .Include(e => e.Requirements)
                .FirstOrDefaultAsync(e =>
                    e.Status == "pending" &&
                    e.EventDate.Date == dto.EventDate.Date &&

                    // exclude self (important if updating)
                    e.Id != id &&

                    // TIME OVERLAP CHECK
                    dto.StartTime < e.EndTime &&
                    dto.EndTime > e.StartTime &&

                    // REQUIREMENT CONFLICT CHECK
                    e.Requirements.Any(r =>
                        r.Type == "non-financial" &&
                        newNonFinancialReqNames.Contains(r.Name)
                    )
                );

            // Step 3: throw exception if conflict found
            if (conflictingEvent != null)
            {
                var conflictingNames = conflictingEvent.Requirements
                    .Where(r =>
                        r.Type == "non-financial" &&
                        newNonFinancialReqNames.Contains(r.Name))
                    .Select(r => r.Name)
                    .Distinct();

                throw new BusinessRuleException(
                    $"Non-financial requirements '{string.Join(", ", conflictingNames)}' " +
                    $"already exist in event '{conflictingEvent.Name}'"
                );
            }

            try
            {
                var existingEvent = await _dB.Events
                    .Include(e => e.Requirements)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (existingEvent == null)
                    throw new NotFoundException("Event not found.");

                // 1️ Update event fields
                existingEvent.Name = dto.Name;
                existingEvent.StartTime = dto.StartTime;
                existingEvent.EndTime = dto.EndTime;
                existingEvent.EventDate = dto.EventDate;
                existingEvent.ReviewMessage = "";

                if (existingEvent.Status == "rejected" || existingEvent.Status == "postponed")
                    existingEvent.Status = "pending";

                // 2️ Delete all old requirements
                if (existingEvent.Requirements.Any())
                {
                    _dB.EventRequirements.RemoveRange(existingEvent.Requirements);
                }

                // 3️ Add all new requirements
                if (dto.Requirements != null && dto.Requirements.Any())
                {
                    foreach (var reqDto in dto.Requirements)
                    {
                        var newReq = new EventRequirement
                        {
                            Type = reqDto.Type,
                            Name = reqDto.Name,
                            Price = reqDto.Price,
                            Quantity = reqDto.Quantity,
                            EventId = existingEvent.Id
                        };

                        await _dB.EventRequirements.AddAsync(newReq);
                    }
                }

                await _dB.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                throw new BusinessRuleException("Unable to update event. Please try again.");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        //Delete Event
        public async Task DeleteEventWithRequirements(Guid eventId)
        {
            using var transaction = await _dB.Database.BeginTransactionAsync();

            try
            {
                var existingEvent = await _dB.Events
                    .Include(e => e.Requirements)
                    .FirstOrDefaultAsync(e => e.Id == eventId);

                if (existingEvent == null)
                    throw new NotFoundException("Event not found.");

                // 1️⃣ Delete all requirements
                if (existingEvent.Requirements.Any())
                {
                    _dB.EventRequirements.RemoveRange(existingEvent.Requirements);
                }

                // 2️⃣ Delete the event
                _dB.Events.Remove(existingEvent);

                await _dB.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                throw new BusinessRuleException("Unable to delete event. Please try again.");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

        }

        // Event history for president
        public async Task<List<Event>> GetEventsHistory(Guid memberId)
        {
            var events = await _dB.Events
                .Include(e => e.Requirements)
                .Where(e => e.Status == "accepted" || e.Status == "postponed")
                .Where(e => _dB.Members
                    .Any(m => m.Id == memberId && m.SocietyId == e.SocietyId))
                .OrderByDescending(e => e.EventDate) // Order by date
                .ToListAsync();

            if (!events.Any())
                throw new NotFoundException("Events not found");

            return events;
        }

        // Update Event status to accept, rejecy or postponed
        public async Task UpdateEventStatus(Guid eventId, UpdateEventStatusDto updateEventStatus)
        {
            var _event = await _dB.Events.FirstOrDefaultAsync(e => e.Id == eventId);

            if (_event == null)
                throw new NotFoundException("Event not found");

            if (!string.IsNullOrEmpty(updateEventStatus.Message))
                _event.ReviewMessage = updateEventStatus.Message;

            _event.Status = updateEventStatus.Status;

            await _dB.SaveChangesAsync();
        }

        // Get specific event by id
        public async Task<Event> GetEventById(Guid eventId)
        {
            var _event = await _dB.Events
                .Include(e => e.Requirements)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if (_event == null)
                throw new NotFoundException("Event not found");

            return _event;
        }

        // Get all reserved non-financial requirements for admin
        //public async Task<List<ViewReservedNonFinancialRequirements>> GetReservedNonFinancialRequirements()
        //{
        //    var eligibleEvent = await _dB.Events
        //        .Include(e => e.Requirements) // Make sure to include requirements
        //        .Where(e => _dB.EventRequisitions
        //            .Any(er => er.EventId == e.Id && (er.Status == "E" || er.Status == "F" || er.Status == "G"))) // Check requisition status for THIS event
        //        .ToListAsync();

        //    if (!eligibleEvent.Any())
        //        throw new NotFoundException("Events not found");

        //    var dto = eligibleEvent.Select(ee => new ViewReservedNonFinancialRequirements()
        //    {
        //        EventName = ee.Name,
        //        EventDate = ee.Date,
        //        NonFinancialRequirements = ee.Requirements
        //            .Where(r => r.Type.Contains("non", StringComparison.OrdinalIgnoreCase)) // Filter only non-financial requirements
        //            .Select(r => new NonFinancialRequirement()
        //            {
        //                ReqName = r.Name,
        //                ReqQty = r.Quantity
        //            }).ToList()
        //    }).ToList();

        //    if (!dto.Any() || dto.All(d => !d.NonFinancialRequirements.Any()))
        //        throw new NotFoundException("Reserved non-financial requirements not found");

        //    return dto;
        //}
    }
}
