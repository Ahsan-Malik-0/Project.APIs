using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Project.APIs.Database;
using Project.APIs.Model;
using Project.APIs.Model.DTOs;

namespace Project.APIs.Services
{
    public class EventRequisitionService(DB _dB)
    {
        public async Task CreateRequisition(CreateEventRequisitionDto newRequisition)
        {
            EventRequisition eventRequisition = new EventRequisition()
            {
                Subject = newRequisition.Subject,
                Body = newRequisition.Body,
                EventId = newRequisition.EventId,
                RequestAmount = newRequisition.RequestedAmount,
                Status = "pending",
            };

            await _dB.EventRequisitions.AddAsync(eventRequisition);
            await _dB.SaveChangesAsync();
        }

        public async Task<List<PendingEventRequisitionsDto>> GetPendingRequisitions(Guid memberId)
        {
            // var eventsIds = await _dB.Events
            //     .Where(e => _dB.Members
            //     .Any(m => m.Id == memberId && e.SocietyId == m.SocietyId))
            //     .Select(e => e.Id)
            //     .ToListAsync();

            // List<EventRequisition> eventRequisitions = new();

            // foreach (var eventId in eventsIds)
            // {
            //     var requisition = await _dB.EventRequisitions
            //         .Where(er => er.Status == "pending")
            //         .FirstOrDefaultAsync(er => er.EventId == eventId);
            //     if (requisition is not null)
            //     {
            //         eventRequisitions.Add(requisition);
            //     }
            // }

            // List<PendingEventRequisitionsDto> pendingEventRequisitionsList = new List<PendingEventRequisitionsDto>();

            // foreach (var requisition in eventRequisitions)
            // {
            //     var requirements = await _dB.EventRequirements
            //     .Where(e => requisition.EventId == e.EventId)
            //     .ToListAsync();

            //     PendingEventRequisitionsDto pendingEventRequisitions = new PendingEventRequisitionsDto()
            //     {
            //         Id = requisition.Id,
            //         Body = requisition.Body,
            //         Subject = requisition.Subject,
            //         Status = requisition.Status,
            //         EventRequirement = _dB.EventRequirements
            //                 .Where(req => req.EventId == requisition.EventId)
            //                 .Select(req => new EventRequirementDto
            //                 {
            //                     Type = req.Type,
            //                     Name = req.Name,
            //                     Quantity = req.Quantity,
            //                     Price = req.Price
            //                 }).ToList()
            //     };

            //     pendingEventRequisitionsList.Add(pendingEventRequisitions);
            // }
            // return pendingEventRequisitionsList;
        

            var result = await _dB.EventRequisitions
            .Where(er => er.Status == "pending" &&
                        _dB.Events.Any(e => e.Id == er.EventId && 
                            _dB.Members.Any(m => m.Id == memberId && 
                                                m.SocietyId == e.SocietyId))) 
            .Select(er => new PendingEventRequisitionsDto
            {
            Body = er.Body,
            Subject = er.Subject,
            Status = er.Status,
            EventRequirement = _dB.EventRequirements
                .Where(req => req.EventId == er.EventId)
                .Select(req => new EventRequirementDto
                {
                    Type = req.Type,
                    Name = req.Name,
                    Quantity = req.Quantity,
                    Price = req.Price
                }).ToList()
            })
            .ToListAsync();

            return result;
        }
    }
    
}
