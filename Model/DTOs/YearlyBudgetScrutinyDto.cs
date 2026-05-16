namespace Project.APIs.Model.DTOs
{
    public class YearlyBudgetScrutinyDto
    {
    }

    public class ViewScrutinyDetailsDto
    {
        public required string AdministrationName { get; set; }
        public required string AdministrationStatus { get; set; }
        public required string AdministrationComment { get; set; }
        public DateTime CommentDate { get; set; }

    }
}
