using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Project.APIs.Model
{
    public class Society
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        public required string Name { get; set; }

        public string? Description { get; set; }

        [JsonIgnore]
        public List<Member>? Members { get; set; }
    }
}