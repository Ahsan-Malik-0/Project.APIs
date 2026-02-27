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

    public class PendingEventRequisitionsDto
    {
        public Guid Id { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public required string Status { get; set; }
        public required ICollection<EventRequirementDto> EventRequirement { get; set; }
    }
}
