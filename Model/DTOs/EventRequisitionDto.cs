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
        public Guid SocietyId { get; set; }
    }
}
