using Microsoft.AspNetCore.Server.HttpSys;
using System.ComponentModel.DataAnnotations;

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
        public required string Subject { get; set; } = string.Empty;
        public required string Body { get; set; } = string.Empty;
        public required DateTime RequestedDate { get; set; }
        public required decimal RequestedAmount { get; set; }
        public required Guid EventId { get; set; }
    }

    public class EventRequisitionPendingDto
    {
        public Guid Id { get; set; }
        public required string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public required string Status { get; set; }
        public string? ReviewMessage { get; set; } // changed by jaosn on 6 march 11:31
    }

    public class SingleEventRequisitionDetailsDto
    {
        public Guid Id { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime RequestedDate { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public required string SocietyName { get; set; }
        public required ICollection<EventRequirementDto> EventRequirements { get; set; }
    }

    public class EventRequisitionHistoryDto
    {
        public Guid Id { get; set; }
        public required string EventName { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime AllocatedDate { get; set; }
        public decimal RequestedAmount { get; set; }
        public decimal AllocatedAmount { get; set; }
        public decimal BiitContribution { get; set; }
    }
}
