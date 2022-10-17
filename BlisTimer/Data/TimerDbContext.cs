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

            modelBuilder.Entity<Activity>()
                .HasOne(x => x.Project)
                .WithMany(x => x.Activities);
            modelBuilder.Entity<TimeLog>()
                .HasOne(x => x.Activity)
                .WithMany()
                .HasForeignKey(x => x.ActivityId);
            modelBuilder.Entity<TimeLog>()
                .HasOne(x => x.Project)
                .WithMany()
                .HasForeignKey(x => x.ProjectId);
            modelBuilder.Entity<Project>()
                .HasMany(x => x.Employees)
                .WithMany(x => x.Projects);


        }

    }
}
