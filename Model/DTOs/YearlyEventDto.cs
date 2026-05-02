namespace Project.APIs.Model.DTOs
{
    public class CreateYearlyEventDto
    {
        public required string Name { get; set; }
        public decimal? EstimateAmount { get; set; }
        public required string EstimateMonth { get; set; }
        public ICollection<CreateYearlyEventRequirementDto>? YearlyEventRequirements { get; set; }
    }
}