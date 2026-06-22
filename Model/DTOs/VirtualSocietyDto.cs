using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Project.APIs.Model.DTOs
{
    public class CreateVirtualSocietyDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime RegistrationEndDate { get; set; }
        public Guid MemberId { get; set; }
    }

    public class GetPastVirtualSocietyDetailsDto
    {
        public required string Name { get; set; }
        public DateTime RegistrationEndDate { get; set; }
        public Guid MemberId { get; set; }
    }

    public class GetPastVirtualSocietyDetailsForSADto : GetPastVirtualSocietyDetailsDto
    {

    }

    public class GetRecentVirtualSocietyDetailsDto : GetPastVirtualSocietyDetailsDto
    {
        public Guid Id { get; set; }
    }

    public class ContributeToVirtualSocietyDto
    {
        public Guid SocietyId { get; set; }
        public required decimal Conrtibution { get; set; }
    }

    public class CreateVirtualSocietyEventsDto
    {
        public List<AddEventDto>? Events { get; set; }
    }

    public class CreateVirtualSocietyRequisitionDto
    {
        public required string Subject { get; set; }
        public required string Body { get; set; }
        public DateTime RequestedDate { get; set; }
        public decimal RequestAmount { get; set; }
        public Guid VirtualSocietyId { get; set; }
        //[JsonIgnore]
        //public VirtualSociety? VirtualSociety { get; set; }
    }



    public class CheckSocietySelectedDto
    {
        public bool IsChecked { get; set; }
    }

    public class AddIdInVirtualSocietiesSocietyDto
    {
        public Guid Id { get; set; }
        public Guid VirtualId { get; set; }
        public List<Guid> SocietyIds { get; set; } = new List<Guid>();
    }

    
}
