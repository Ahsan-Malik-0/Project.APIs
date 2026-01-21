using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Project.APIs.Model.DTOs
{
    public class MemberDto
    {

        [StringLength(20)]
        [Required(ErrorMessage = "Name is required")]
        public required string Name { get; set; }

        [StringLength(20)]
        [Required(ErrorMessage = "Username is required")]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public required string HashPassword { get; set; }

        [Required(ErrorMessage = "Role is required")]
        public required string Role { get; set; }

        [Required(ErrorMessage = "Picture is required")]
        public required string Picture { get; set; }

        [Required(ErrorMessage = "Society Id is required")]
        public Guid SocietyId { get; set; }
    }

    public class MemberLoginDto
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(20)]
        public required string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public required string HashPassword { get; set; }
    }

    public class UpdateMemberProfileDto
    {
        [Required(ErrorMessage = "Id is required")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public required string Name { get; set; }
        [StringLength(20)]

        [Required(ErrorMessage = "Username is required")]
        public required string Username { get; set; }

        //[JsonIgnore]
        [Required(ErrorMessage = "OldPassword is required")]
        public required string OldHashPassword { get; set; }

        [Required(ErrorMessage = "NewPassword is required")]
        public required string NewHashPassword { get; set; }
        public required string Picture { get; set; }
    }
}
