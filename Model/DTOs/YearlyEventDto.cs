namespace Project.APIs.Model.DTOs
{
    public class YearlyEventResponseDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal? EstimateAmount { get; set; }
        public string EstimateMonth { get; set; } = string.Empty;
        public List<YearlyEventRequirementResponseDto>? YearlyEventRequirements { get; set; }
    }

    public class CreateYearlyEventDto
    {
        public required string Name { get; set; }
        public decimal? EstimateAmount { get; set; }
        public required string EstimateMonth { get; set; }
        public ICollection<CreateYearlyEventRequirementDto>? YearlyEventRequirements { get; set; }
    }
}