using System.Text.Json.Serialization;

namespace Project.APIs.Model
{
    public class EventRequisition
    {
        public Guid Id { get; set; }
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public required string Status { get; set; }
        public DateTime AllocatedDate { get; set; }
        public decimal RequestAmount { get; set; }
        public decimal AllocatedAmount { get; set; }
        public decimal BiitContribution { get; set; }
        public Guid SocietyId { get; set; }
        [JsonIgnore]
        public Event? _event { get; set; }
    }
}
