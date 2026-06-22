namespace Project.APIs.Model
{
    public class VirtualSociety
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal TotalContribution { get; set; }
        public DateTime RegistrationEndDate { get; set; }
        public Guid MemberId { get; set; }
        public Member? Member { get; set; }
    }
}
