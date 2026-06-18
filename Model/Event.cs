using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Project.APIs.Model
{
    public class Event
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public DateTime EventDate { get; set; }
        public required string Status { get; set; }
        public string? ReviewMessage { get; set; }
        public Guid SocietyId { get; set; }
        public Society? Society { get; set; }
        public Guid? RequisitionId { get; set; }
        [JsonIgnore]
        [ForeignKey(nameof(RequisitionId))] // Add this line
        public EventRequisition? EventRequisition { get; set; }
        public required IList<EventRequirement> Requirements { get; set; }
    }
}
