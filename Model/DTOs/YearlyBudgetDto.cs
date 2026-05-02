namespace Project.APIs.Model.DTOs
{
    public class CreateYearlyBudgetDto
    {
        public required string Session { get; set; }
        public required decimal RequestedAmount { get; set; }
        public required DateTime RequestedDate { get; set; }
        public ICollection<CreateYearlyEventDto>? YearlyEvents { get; set; }
    }


}