using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Project.APIs.Model
{
    public class Member
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Username { get; set; }
        public string? HashPassword { get; set; }
        public required string Role { get; set; }
        public string? ProfileImage { get; set; }
        public Guid? SocietyId { get; set; }
        [JsonIgnore]
        public Society? Society { get; set; }
    }
}
