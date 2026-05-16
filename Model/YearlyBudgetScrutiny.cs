namespace Project.APIs.Model
{
    public class YearlyBudgetScrutiny
    {
        public Guid Id { get; set; }
        public required string Comment { get; set; }
        public string? Status { get; set; }
        public DateTime Date { get; set; }
        public Guid AdministrationId { get; set; }
        public Administration? Administration { get; set; }
        public Guid YearlyBydgetId { get; set; }
        public YearlyBudget? YearlyBudget { get; set; }
    }
}
