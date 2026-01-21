using System.ComponentModel.DataAnnotations;

namespace Project.APIs.Model
{
    public class SocietyDto
    {
        [Required(ErrorMessage = "Name is required")]
        [StringLength(50)]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(2000)]
        public required string Description { get; set; }
    }
}
