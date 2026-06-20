
using System.ComponentModel.DataAnnotations;

namespace Project.APIs.Model.DTOs
{
    public class CreateEventRequisitionDto
    {
        public required string Subject { get; set; } = string.Empty;
        public required string Body { get; set; } = string.Empty;
        public required DateTime RequestedDate { get; set; }
        public required decimal RequestedAmount { get; set; }
        public required Guid EventId { get; set; }
    }

    public class CreateVirtualEventRequisitionDto
    {
        public required string Subject { get; set; } = string.Empty;
        public required string Body { get; set; } = string.Empty;
        public required DateTime RequestedDate { get; set; }
        public required decimal RequestedAmount { get; set; }
        public required ICollection<Guid> EventIds { get; set; }
    }

    public class UpdateEventRequisitionDto
    {
        public required string Subject { get; set; } = string.Empty;
        public required string Body { get; set; } = string.Empty;
        public required DateTime RequestedDate { get; set; }
        public required decimal RequestedAmount { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DateTime EventDate { get; set; }
        public required ICollection<EventRequirementDto> EventRequirements { get; set; }
    }

    public class UpdateVirtualEventRequisitionDto
    {
        public required string Subject { get; set; } = string.Empty;
        public required string Body { get; set; } = string.Empty;
        public required DateTime RequestedDate { get; set; }
        public required decimal RequestedAmount { get; set; }
        public required ICollection<Guid> EventIds { get; set; }
    }


    // For pending requisition list
    public class RequisitionDetailsForChairperson
    {
        public Guid Id { get; set; }
        public DateTime RequestedDate { get; set; }
        public decimal RequestedAmount { get; set; }
        public required string Status { get; set; }
        public string? ReviewMessage { get; set; } // changed by jaosn on 6 march 11:31
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public required string ChairpersonName { get; set; }
        public required Event Event { get; set; }
    }

    public class RequisitionDetailsForAdministration : RequisitionDetailsForChairperson { }


    public class VirtualRequisitionDetailsForChairperson
    {
        public Guid Id { get; set; }
        public DateTime RequestedDate { get; set; }
        public decimal RequestedAmount { get; set; }
        public required string Status { get; set; }
        public string? ReviewMessage { get; set; } // changed by jaosn on 6 march 11:31
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public required string ChairpersonName { get; set; }
        public required List<Event> Events { get; set; }
    }

    // For more details after selecting a specific requisition
    public class EventRequisitionDetailsDto
    {
        public Guid Id { get; set; }
        public DateTime EventDate { get; set; }
        public required ICollection<EventRequirementDto> EventRequirements { get; set; }
        public required string SocietyName { get; set; }
    }

    public class EventRequisitionHistoryDto
    {
        public Guid RequisitionId { get; set; }
        public Guid EventId { get; set; }
        public required string EventName { get; set; }
        public required string RequisitionStatus { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime AllocatedDate { get; set; }
        public decimal RequestedAmount { get; set; }
        public decimal AllocatedAmount { get; set; }
        public decimal BiitContribution { get; set; }
    }

    // For student affairs and administration to view details of a pending requisition
    public class ViewRequisitionRequestDetailsDto
    {
        public Guid Id { get; set; }
        public required string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime RequestedDate { get; set; }
        public required string SocietyName { get; set; }
        public decimal RequestedAmount { get; set; }
        public decimal AllotedAmount { get; set; }
        public decimal BiitContribution { get; set; }
    }

    public class ReviewEventRequisitionDto
    {
        public required string Status { get; set; }
        public string? ReviewMessage { get; set; }
    }

    public class ApproveEventRequisitionDto
    {
        public DateTime AllocatedDate { get; set; }
        public required decimal AllocatedAmount { get; set; }
        public required decimal BiitContribution { get; set; }
    }

    public class ViewRequisitionDetailsForFinanceDto
    {
        public Guid RequisitionId { get; set; }
        public string? ChairpersonName { get; set; }
        public required string SocietyName { get; set; }
        public required string EventName { get; set; }
        public DateTime EventDate { get; set; }
        public decimal AllotedBudget { get; set; }
        public decimal BiitContribution { get; set; }
    }

    public class ViewRequisitionDetailsForFinanceHistoryDto : ViewRequisitionDetailsForFinanceDto
    {
        public Guid EventId { get; set; }
        public required string Status { get; set; }
        
    }
    public class ViewRequisitionDetailsForStudentAffairsDto : ViewRequisitionDetailsForFinanceDto 
    {
        public required string Status { get; set; }
        public string? ReviewMessage { get; set; }
    }

    public class ResponseMessageDto
    {
        public required string ResponseMessage { get; set; }
    }


    public class RequisitionDetailsForYearlyBudgetDto
    {
        public decimal AllotedAmount { get; set; }
    }
    
}
