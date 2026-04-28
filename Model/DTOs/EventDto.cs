using System.ComponentModel.DataAnnotations;

namespace Project.APIs.Model.DTOs
{
    public class EventDto
    {
        [Required(ErrorMessage = "Name is required")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public DateTime Date { get; set; }

        public string? Status { get; set; }

        public string? Message { get; set; }

        [Required(ErrorMessage = "SocietyId is required")]
        public Guid SocietyId { get; set; }
    }

    public class AddEventDto
    {
        [Required(ErrorMessage = "Name is required")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "SocietyId is required")]
        public Guid SocietyId { get; set; }

        [Required(ErrorMessage = "Requirements are required")]
        public required ICollection<EventRequirementDto> Requirements { get; set; }
    }

    public class UpdateEventDto
    {
        [Required(ErrorMessage = "Name is required")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Requirements are required")]
        public required ICollection<EventRequirementDto> Requirements { get; set; }
    }

    public class UpdateEventStatusDto
    {

        [Required(ErrorMessage = "Status is required")]
        public required string Status { get; set; }
        public string? Message { get; set; }
    }
}
