using Microsoft.EntityFrameworkCore;
using Project.APIs.Model;

namespace Project.APIs.Database
{
    public class DB(DbContextOptions<DB> options) : DbContext(options)
    {
        public DbSet<Society> Societies { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventRequirement> EventRequirements { get; set; }
    }
}