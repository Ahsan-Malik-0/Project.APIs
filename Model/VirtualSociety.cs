namespace Project.APIs.Model
{
    public class VirtualSociety
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Session { get; set; }
        public string? Description { get; set; }
        public Guid ChairpersonId { get; set; }
        public Member? Member { get; set; }
        public ICollection<Event>? Events { get; set; }
    }
}
