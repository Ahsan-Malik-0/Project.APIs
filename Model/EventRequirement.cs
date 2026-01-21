using System.Text.Json.Serialization;

namespace Project.APIs.Model
{
    public class EventRequirement
    {
        public int Id { get; set; }
        public required string Type { get; set; }
        public required string Name { get; set; }

        public float Price { get; set; }
        public int Quantity { get; set; }

        public Guid EventId { get; set; }

        [JsonIgnore]
        public Event? _event { get; set; }

    }
}
