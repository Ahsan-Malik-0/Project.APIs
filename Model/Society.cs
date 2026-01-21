using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Project.APIs.Model
{
    public class Society
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(50)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(2000)]
        public required string Description { get; set; }

        // Navigation to members so EF can discover the relationship from the other side too
        [JsonIgnore]
        public virtual ICollection<Member> Members { get; set; } = new List<Member>();
    }
}