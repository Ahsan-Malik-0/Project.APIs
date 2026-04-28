using Microsoft.EntityFrameworkCore;
using Project.APIs.Database;
using Project.APIs.Exceptions;
using Project.APIs.Model;
using Project.APIs.Model.DTOs;

namespace Project.APIs.Services
{
    public class EventRequisitionService(DB _dB)
    {
        public async Task CreateRequisition(CreateEventRequisitionDto newRequisition)
        {
            var @event = await _dB.Events.FirstOrDefaultAsync(e => e.Id == newRequisition.EventId);

            if(@event == null)
                throw new NotFoundException("Event Not Found");

            @event.Status = "accepted";

            EventRequisition eventRequisition = new EventRequisition()
            {
                RequestedDate = newRequisition.RequestedDate,
                Subject = newRequisition.Subject,
                Body = newRequisition.Body,
                EventId = newRequisition.EventId,
                RequestAmount = newRequisition.RequestedAmount,
                Status = "pending",
            };

            await _dB.EventRequisitions.AddAsync(eventRequisition);
            await _dB.SaveChangesAsync();
        }


        //public async Task<List<PendingEventRequisitionsDto>> GetPendingRequisitions(Guid memberId)
        // Member Id In
        // Event name
        // event date
        // Status
        // Review Message (if any)

        public async Task<List<EventRequisitionPendingDto>> GetPendingEventRequisitions(Guid memberId)
        {
            // By joining method
            //var result = await (
            //    from er in _dB.EventRequisitions
            //    join e in _dB.Events on er.EventId equals e.Id
            //    join m in _dB.Members on e.SocietyId equals m.SocietyId
            //    where er.Status == "pending"
            //          && m.Id == memberId
            //    select new PendingEventRequisitionDetailsDto
            //    {
            //        Id = er.EventId,
            //        EventName = e.Name,
            //        EventDate = e.Date,
            //        Status = er.Status
            //    }
            //)
            //.AsNoTracking()
            //.ToListAsync();

            // By sub query method
            //var result = await _dB.EventRequisitions
            //    .Where(er => er.Status == "pending" &&
            //        _dB.Events.Any(e => e.Id == er.EventId &&
            //                _dB.Members.Any(m => m.Id == memberId &&
            //                                    m.SocietyId == e.SocietyId)))
            //    .Select(er => new PendingEventRequisitionDetailsDto()
            //    {
            //      Id = er.Id, /// changed by jason  on 6 march 11:30
            //      EventName = _dB.Events.Where(e => e.Id == er.EventId).Select(e => e.Name).FirstOrDefault()!,
            //      EventDate = _dB.Events.Where(e => e.Id == er.EventId).Select(e => e.Date).FirstOrDefault(),
            //      Status = er.Status,
            //      ReviewMessage = er.ReviewMessage /// changed by jason  on 6 march 11:32
            //    })
            //    .AsNoTracking()
            //    .ToListAsync();

            // By chaining method
            var result = await _dB.EventRequisitions
                .Where(er => er.Status == "pending"
                    && er._event!.Society!.Members.Any(m => m.Id == memberId))
                .Select(er => new EventRequisitionPendingDto()
                {
                    Id = er.Id,
                    EventName = er._event!.Name,
                    EventDate = er._event.Date,
                    Status = er.Status,
                    ReviewMessage = er.ReviewMessage
                })
                .AsNoTracking()
                .ToListAsync();

            return result;
        }


        public async Task<SingleEventRequisitionDetailsDto> GetEventRequisitionDetails(Guid requisitionId)
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


            //var result = await _dB.EventRequisitions
            //.Where(er => er.Status == "pending" &&
            //            _dB.Events.Any(e => e.Id == er.EventId && 
            //                _dB.Members.Any(m => m.Id == memberId && 
            //                                    m.SocietyId == e.SocietyId))) 
            //.Select(er => new EventRequisitionDetailsDto
            //{
            //    Id = er.Id,
            //    Body = er.Body,
            //    Subject = er.Subject,
            //    Status = er.Status,
            //    EventRequirements = _dB.EventRequirements
            //        .Where(req => req.EventId == er.EventId)
            //        .Select(req => new EventRequirementDto
            //        {
            //            Type = req.Type,
            //            Name = req.Name,
            //            Quantity = req.Quantity,
            //            Price = req.Price
            //        }).ToList()
            //})
            //.ToListAsync();

            var result = await _dB.EventRequisitions
                .Where(er => er.Id == requisitionId)
                .Select(er => new SingleEventRequisitionDetailsDto()
                {
                    Id = er.Id,
                    EventDate = _dB.EventRequisitions
                            .Where(er => er.Id == requisitionId)
                            .Select(er => er._event!.Date)
                            .FirstOrDefault(),
                    RequestedDate = er.RequestedDate,
                    Subject = er.Subject,
                    Body = er.Body,
                    SocietyName = _dB.EventRequisitions
                            .Where(er => er.Id == requisitionId)
                            .Select(er => er._event!.Society!.Name)  // If navigation properties are set up
                            .FirstOrDefault()!,
                    EventRequirements = _dB.EventRequirements
                            .Where(req => req.EventId == er.EventId)
                            .Select(req => new EventRequirementDto
                            {
                                Type = req.Type,
                                Name = req.Name,
                                Quantity = req.Quantity,
                                Price = req.Price
                            }).ToList()
                })
                .FirstOrDefaultAsync();

            if (result == null)
                throw new NotFoundException("Requisition Not Found");

            return result;
        }

        public async Task<List<EventRequisitionHistoryDto>> GetEventRequisitionHistory(Guid memberId)
        {
            var societyId = await _dB.Members
                    .Where(m => memberId == m.Id)
                    .Select(m => m.SocietyId)
                    .FirstOrDefaultAsync();

            if (societyId == Guid.Empty)
                throw new NotFoundException("Society not found");

            var acceptedRequisition = await _dB.EventRequisitions
                    .Where(er => er.Status == "budgetAllocated"
                    && er._event!.SocietyId == societyId)
                    .Select(er => new EventRequisitionHistoryDto()
                    {
                        Id = er.Id,
                        EventName = er._event!.Name,
                        RequestedDate = er.RequestedDate,
                        AllocatedDate = er.AllocatedDate,
                        RequestedAmount = er.RequestAmount,
                        AllocatedAmount = er.RequestAmount,
                        BiitContribution = er.BiitContribution
                    })
                    .AsNoTracking()
                    .ToListAsync();

            return acceptedRequisition;
        }
    }
    
}
