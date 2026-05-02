using Microsoft.EntityFrameworkCore;

namespace Project.APIs.Model
{
    [Index(nameof(Session), IsUnique = true)]
    public class YearlyBudget
    {
        public Guid Id { get; set; }
        public required string Session { get; set; }
        public required decimal RequestedAmount { get; set; }
        public decimal AllotedAmount { get; set; }
        public required DateTime RequestedDate { get; set; }
        public DateTime AllotedDate { get; set; }
        public decimal Credits { get; set; }
        public Guid SocietyId { get; set; }
        public Society? Society { get; set; }

        public ICollection<YearlyEvent>? YearlyEvents { get; set; }
    }

}
