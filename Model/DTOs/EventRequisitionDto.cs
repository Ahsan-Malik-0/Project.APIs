using Microsoft.AspNetCore.Server.HttpSys;

namespace Project.APIs.Model.DTOs
{
    public class EventRequisitionDto
    {
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public Guid SocietyId { get; set; }
    }

    public class CreateEventRequisitionDto
    {
        public DateTime? ReqestedDate { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public decimal RequestedAmount { get; set; }
        public Guid EventId { get; set; }
    }

    public class RequisitoinDetailForChairperosnDto
    {
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public required string Status { get; set; }
        public Guid EventId { get; set; }
        public required ICollection<EventRequirementDto> EventRequirement { get; set; }
        
    }

    public class EventRequisitionDetailsDto
    {
        public Guid Id { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public required ICollection<EventRequirementDto> EventRequirements { get; set; }
    }

    public class PendingEventRequisitionDetailsDto
    {
        public Guid Id { get; set; }
        public required string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public required string Status { get; set; }
        public string? ReviewMessage { get; set; } // changed by jaosn on 6 march 11:31
    }
}
