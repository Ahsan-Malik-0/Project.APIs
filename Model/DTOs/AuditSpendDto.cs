namespace Project.APIs.Model.DTOs
{
    public class CreateAuditSpendDto
    {
        public required string VenderName { get; set; }
        public string? ItemDescription { get; set; }
        public required decimal Amount { get; set; }
        public string? ReceiptPiture { get; set; }
    }

    public class UpdateAuditSpendDto
    {
        public required string VenderName { get; set; }
        public string? ItemDescription { get; set; }
        public required decimal Amount { get; set; }
        public string? ReceiptPiture { get; set; }
    }
}
