using System.Text.Json.Serialization;

namespace Project.APIs.Model
{
    public class AuditSpend
    {
        public Guid Id { get; set; }
        public required string Vender { get; set; }
        public string? Description { get; set; }
        public required decimal Amount { get; set; }
        public string? ReceiptPiture { get; set; }

        public Guid AuditId;

        [JsonIgnore]
        public EventAudit? EventAudit { get; set; }
    }
}
