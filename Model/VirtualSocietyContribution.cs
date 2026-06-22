using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace Project.APIs.Model
{
    public class VirtualSocietyContribution
    {
        public Guid Id { get; set; }
        public decimal Contribution {  get; set; }

        public Guid VirtualSocietyId { get; set; }
        [JsonIgnore]
        public VirtualSociety? VirtualSociety { get; set; }

        public Guid SocietyId { get; set; }
        [JsonIgnore]
        public Society? Society { get; set; }
    }
}
