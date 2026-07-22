using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Project.APIs.Model.DTOs
{
    //public class CreateVirtualSocietyDto
    //{
    //    public required string Name { get; set; }
    //    public string? Description { get; set; }
    //    public DateTime RegistrationEndDate { get; set; }
    //    public Guid MemberId { get; set; }
    //
    //}



    public class CreateVirtualSocietyDto
    {
        public required string Name { get; set; }
        public string? Description { get; set; }
        public DateTime RegistrationEndDate { get; set; }

        public Guid ChairpersonId { get; set; }

        public List<Guid>? SocietyIds { get; set; }
    }

    public class GetVirtualSocietyDetailsDto
    {
        public Guid VirtualSocietyId { get; set; }
        public required string VirtualSocietyName { get; set; }
        public DateTime RegistrationEndDate { get; set; }
        public decimal TotalContribution { get; set; }
        public Guid ManagerId { get; set; }  
        public List<Event>? VirtualSocietyEvents { get; set; }
        public List<ContributedSocietiesDto>? ContributedSocieties { get; set; }
        public EventRequisition? VirtualSocietyRequisition { get; set; }
    }

    public class GetVirtualSocietyDetailsForFinanceDto
    {
        public Guid VirtualSocietyId { get; set; }
        public required string VirtualSocietyName { get; set; }
        public decimal TotalContribution { get; set; }
        public List<ContributedSocietiesDto>? ContributedSocieties { get; set; }
    }

    public class ContributedSocietiesDto
    {
        public string? SocietyName { get; set; }
        public Guid Chairpersonid { get; set; }
        public decimal Conrtibution { get; set; }
    }

    public class ContributeToVirtualSocietyDto
    {
        public Guid VirtualSocietyId { get; set; }
        public decimal Contribution { get; set; }
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
        public decimal RequestedAmount { get; set; }
        public List<Guid>? EventIds { get; set; } 
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
