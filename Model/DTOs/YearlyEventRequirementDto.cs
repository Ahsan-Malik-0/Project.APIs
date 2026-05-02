namespace Project.APIs.Model.DTOs
{
    public class CreateYearlyEventRequirementDto
    {
        public required string Name { get; set; }
        public decimal? EstimatePrice { get; set; }
        public YearlyEvent? YearlyEvent { get; set; }
    }
}