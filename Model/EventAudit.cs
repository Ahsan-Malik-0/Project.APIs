using System.ComponentModel.DataAnnotations.Schema;
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
        public Guid RequisitionId { get; set; }

        [ForeignKey(nameof(RequisitionId))]
        [JsonIgnore]
        public EventRequisition? EventRequisition { get; set; }

        public virtual ICollection<AuditSpend>? Spends { get; set; }
    }
}
