using System.ComponentModel.DataAnnotations;

namespace Project.APIs.Model.DTOs
{
    public class EventRequirementDto
    {
        [Required(ErrorMessage = "Type in required")]
        public required string Type { get; set; }

        [Required(ErrorMessage = "Type in required")]
        public required string Name { get; set; }

        public float Price { get; set; }
        public int Quantity { get; set; }
    }
}
