namespace Project.APIs.Model
{
    public class YearlyBudgetScrutiny
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Comment { get; set; }
        public DateTime Date { get; set; }
        public Guid MemberId { get; set; }
        public Member? Member { get; set; }
        public Guid YearlyBudgetId { get; set; }
        public YearlyBudget? YearlyBudget { get; set; }
    }
}
