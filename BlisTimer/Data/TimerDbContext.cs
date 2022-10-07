using BlisTimer.Models;
using Microsoft.EntityFrameworkCore;

namespace BlisTimer.Data
{
    public class TimerDbContext : DbContext
    {

        public TimerDbContext(DbContextOptions<TimerDbContext> options) : base(options){ }

        public DbSet<Activity> Activities { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<TimeLog> TimeLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseSerialColumns();
        }

    }
}
