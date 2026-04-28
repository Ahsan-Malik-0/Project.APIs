using System.Text.Json.Serialization;

namespace Project.APIs.Model
{
    public class EventAudit
    {
        public Guid Id { get; set; }
        public decimal FundProvided { get; set; }
        public decimal SpendAmount { get; set; }
        public decimal RevenueGenerated { get; set; }
        public decimal RemainingAmount { get; set; }
        public string? Status { get; set; }
        public Guid EventId { get; set; }
        [JsonIgnore]
        public Event? _event { get; set; }

        public virtual ICollection<AuditSpend>? Spends { get; set; }
    }
}
