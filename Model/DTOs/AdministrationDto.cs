
namespace Project.APIs.Model.DTOs
{

    public class AdministrationDto
    {
        public class CreateAdministrationDto
        {
            public required string Name { get; set; }
            public required string Username { get; set; }
            public required string HashPassword { get; set; }
            public required string Role { get; set; }
            public string? Picture { get; set; }
        }
        public class AdministrationLoginDto
        {
            public required string Username { get; set; }
            public required string HashPassword { get; set; }
        }


        public class AdminProfileDto
        {
            public required string Name { get; set; }
            public required string Username { get; set; }
            public string? Picture { get; set; }
        }

        public class UpdateAdminProfileDto
        {
            public Guid Id { get; set; }
            public required string Name { get; set; }
            public required string Username { get; set; }
            public string? OldHashPassword { get; set; }
            public string? NewHashPassword { get; set; }
            public string? Picture { get; set; }
        }

    }
}
