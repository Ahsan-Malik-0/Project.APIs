using System.Diagnostics.Contracts;

namespace Project.APIs.Model.DTOs
{
    public class YearlyBudgetScrutinyDto
    {
        public required string AdministrationName { get; set; }
        // public required string AdministrationStatus { get; set; }
        public required string AdministrationComment { get; set; }
        public required string AdministrationRole { get; set; }
        public DateTime CommentDate { get; set; }
    }

    public class ViewScrutinyDetailsDto : YearlyBudgetScrutinyDto
    {
        public Guid Id { get; set; }
        
    }

    public class AddCommentDto : YearlyBudgetScrutinyDto
    {
        public Guid YearlyBudgetId { get; set; } 
    }
}
