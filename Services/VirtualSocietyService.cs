using Microsoft.EntityFrameworkCore;
using Project.APIs.Database;
using Project.APIs.Exceptions;
using Project.APIs.Model;
using Project.APIs.Model.DTOs;

namespace Project.APIs.Services
{
    public class VirtualSocietyService(DB _dB)
    {
        public async Task CreateEventRequisition(CreateVirtualEventRequisitionDto newRequisition)
        {
            using var transaction = await _dB.Database.BeginTransactionAsync();
            try
            {

                EventRequisition eventRequisition = new EventRequisition()
                {
                    RequestedDate = newRequisition.RequestedDate,
                    Subject = newRequisition.Subject,
                    Body = newRequisition.Body,
                    //EventId = newRequisition.EventId,
                    RequestAmount = newRequisition.RequestedAmount,
                    Status = "A",
                    Events = null
                };

                await _dB.EventRequisitions.AddAsync(eventRequisition);
                await _dB.SaveChangesAsync();

                foreach (Guid eventId in newRequisition.EventIds)
                {
                    var _event = await _dB.Events.FirstOrDefaultAsync(e => e.Id == eventId);
                    if (_event == null)
                    {
                        throw new NotFoundException("Event Not Found");
                    }

                    _event.Status = "accepted";
                    _event.RequisitionId = eventRequisition.Id;

                }

                await _dB.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (DbUpdateException)
            {
                await transaction.RollbackAsync();
                throw new BusinessRuleException("Unable to save requisition. Please try again.");
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw; // goes to 500 handler
            }
        }

        public async Task UpdateEventRequisition(Guid requisitionId, UpdateVirtualEventRequisitionDto updateEventRequisition)
        {
            var transaction = await _dB.Database.BeginTransactionAsync();
            try
            {

                var requisition = await _dB.EventRequisitions.FindAsync(requisitionId);

                if (requisition == null)
                    throw new NotFoundException("Requisition Not Found");

                requisition.Subject = updateEventRequisition.Subject;
                requisition.Body = updateEventRequisition.Body;
                requisition.RequestedDate = updateEventRequisition.RequestedDate;
                requisition.RequestAmount = updateEventRequisition.RequestedAmount;

                if (requisition.Status == "B") requisition.Status = "A";
                if (requisition.Status == "D") requisition.Status = "C";
                if (requisition.Status == "F") requisition.Status = "E";

                // Remove requisition id from old events
                var oldEventList = await _dB.Events.Where(e => e.RequisitionId == requisitionId).ToListAsync();

                foreach (var _event in oldEventList)
                {
                    _event.RequisitionId = null;
                    _event.Status = "pending";
                }

                // Add requisition id in new events
                foreach (Guid eventId in updateEventRequisition.EventIds)
                {
                    var _event = await _dB.Events.FirstOrDefaultAsync(e => e.Id == eventId);
                    if (_event == null)
                    {
                        throw new NotFoundException("Event Not Found");
                    }

                    _event.Status = "accepted";
                    _event.RequisitionId = requisitionId;
                }

                _dB.EventRequisitions.Update(requisition);
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

        public async Task<List<VirtualRequisitionDetailsForChairperson>> GetRequisitionDetailsForChairperson(Guid memberId)
        {
            var result = await _dB.EventRequisitions
                .Where(er => er.Status != "H" && er.Status != "I" && er.Status != "J")
                .Where(er => er.Events!.FirstOrDefault()!.Society!.Members!.Any(m => m.Id == memberId))
                .Select(er => new
                {
                    er.Id,
                    er.RequestedDate,
                    er.RequestAmount,
                    er.Status,
                    er.ReviewMessage,
                    er.Subject,
                    er.Body,
                    Events = er.Events!.Select(e => new
                    {
                        e.Id,
                        e.Name,
                        Requirements = e.Requirements.ToList(),
                        Society = new
                        {
                            e.Society!.Id,
                            e.Society.Name,
                            Member = e.Society.Members!.FirstOrDefault(m => m.Id == memberId)!.Name
                        }
                    }).ToList()
                })
                .AsNoTracking()
                .ToListAsync();

            string member = result!.FirstOrDefault()!.Events!.FirstOrDefault()!.Society.Member;

            return result.Select(er => new VirtualRequisitionDetailsForChairperson()
            {
                Id = er.Id,
                RequestedDate = er.RequestedDate,
                RequestedAmount = er.RequestAmount,
                Status = StatusMap.GetValueOrDefault(er.Status, "Unknown"),
                ReviewMessage = er.ReviewMessage,
                Subject = er.Subject,
                Body = er.Body,
                ChairpersonName = member,
                Events = er.Events.Select(e => new Event
                {
                    Id = e.Id,
                    Name = e.Name,
                    Status = null!,
                    Requirements = e.Requirements,
                    Society = new Society
                    {
                        Id = e.Society.Id,
                        Name = e.Society.Name,
                        Members = new List<Member>
                        {
                            new Member { Name = e.Society.Member, Username = null!, Role = null! }
                        }
                    }
                }).ToList()
            }).ToList();
        }

        private static readonly Dictionary<string, string> StatusMap = new()
        {
            ["A"] = "Pending",
            ["B"] = "Reject By Student Affairs",
            ["C"] = "Approved By Student Affairs",
            ["D"] = "Reject By Admin",
            ["E"] = "Approved By Admin",
            ["F"] = "Reject By Finance",
            ["G"] = "Budget Released By Finance",
            ["H"] = "Event Completed",
            ["I"] = "Request For Audit",
            ["J"] = "Audit Cleared"
        };
    }
}