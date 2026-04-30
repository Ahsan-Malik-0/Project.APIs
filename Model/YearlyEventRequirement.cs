namespace Project.APIs.Model
{
    public class YearlyEventRequirement
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public decimal? EstimatePrice { get; set; }

        public Guid YearlyEventId { get; set; }
        public YearlyEvent? YearlyEvent { get; set; }
    }
}
