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
                var _event = new Event
                {
                    Name = newEvent.Name,
                    Date = newEvent.Date,
                    Status = status,
                    Message = null,
                    SocietyId = newEvent.SocietyId
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

            try
            {
                var existingEvent = await _dB.Events
                    .Include(e => e.Requirements)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (existingEvent == null)
                    throw new NotFoundException("Event not found.");

                // 1️ Update event fields
                existingEvent.Name = dto.Name;
                existingEvent.Date = dto.Date;

                if (existingEvent.Status == "rejected")
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

        // Get all accepted events
        public async Task<List<Event>> GetEventsHistory(Guid memberId)
        {
            var events = await _dB.Events
                .Include(e => e.Requirements)
                .Where(e => e.Status == "accepted" || e.Status == "waiting")
                .Where(e => _dB.Members
                    .Any(m => m.Id == memberId && m.SocietyId == e.SocietyId))
                .OrderByDescending(e => e.Date) // Order by date
                .ToListAsync();

            if (!events.Any())
                throw new NotFoundException("Events not found");

            return events;
        }

        // Update Event status to accept, rejecy or waiting
        public async Task UpdateEventStatus(UpdateEventStatusDto updateEventStatus)
        {
            var _event = await _dB.Events.FirstOrDefaultAsync(e => e.Id == updateEventStatus.Id);

            if (_event == null)
                throw new NotFoundException("Event not found");

            if(!string.IsNullOrEmpty(updateEventStatus.Message))
                _event.Message = updateEventStatus.Message;

            _event.Status = updateEventStatus.Status;

            await _dB.SaveChangesAsync();
        }

        // Get specific event by id
        public async Task<Event> GetEventById(Guid eventId)
        {
            var _event = await _dB.Events
                .Include(e => e.Requirements)
                .FirstOrDefaultAsync(e => e.Id == eventId);

            if(_event == null)
                throw new NotFoundException("Event not found");

            return _event;
        }
    }
}
