using System.Text.Json.Serialization;

namespace Project.APIs.Model.DTOs
{
    public class CreateEventAuditDto
    {
        public required decimal FundProvided { get; set; }
        public required decimal SpendAmount { get; set; }
        public required decimal RevenueGenerated { get; set; }
        public required decimal RemainingAmount { get; set; }
        public required Guid EventId { get; set; }
        public required ICollection<CreateAuditSpendDto> Spends { get; set; }
    }

    public class UpdateEventAuditDto
    {
        public required decimal FundProvided { get; set; }
        public required decimal SpendAmount { get; set; }
        public required decimal RevenueGenerated { get; set; }
        public required decimal RemainingAmount { get; set; }
        public required ICollection<UpdateAuditSpendDto> Spends { get; set; }
    }
}
