namespace Project.APIs.Model.DTOs
{
    public class YearlyEventRequirementResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal? EstimatePrice { get; set; }
    }

    public class CreateYearlyEventRequirementDto
    {
        public required string Name { get; set; }
        public decimal? EstimatePrice { get; set; }
    }
}