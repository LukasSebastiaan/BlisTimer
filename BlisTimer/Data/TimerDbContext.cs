using BlisTimer.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

namespace BlisTimer.Data
{
    public class TimerDbContext : DbContext
    {
        
        public TimerDbContext(DbContextOptions<TimerDbContext> options) : base(options){ }

        public DbSet<WorkActivity> WorkActivities { get; set; } = null!;
        public DbSet<Project> Projects { get; set; } = null!;
        public DbSet<TimeLog> TimeLogs { get; set; } = null!;
        public DbSet<Employee> Employees { get; set; } = null!;
        public DbSet<HourType> HourTypes { get; set; } = null!;
        
        public DbSet<EmployeeProject> EmployeeProjects { get; set; } = null!;
        public DbSet<WorkActivityHourType> WorkActivityHourTypes { get; set; } = null!;
        public DbSet<RunningTimer> RunningTimers { get; set; } = null!;
        
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseSerialColumns();

            modelBuilder.Entity<RunningTimer>()
                .HasOne(_ => _.Employee)
                .WithOne(_ => _.RunningTimer)
                .HasForeignKey<RunningTimer>(_ => _.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<EmployeeProject>()
                .HasKey(ep => new { ep.EmployeeId, ep.ProjectId });
            modelBuilder.Entity<EmployeeProject>()
                .HasOne(ep => ep.Employee)
                .WithMany(p => p.EmployeeProjects)
                .HasForeignKey(ep => ep.EmployeeId);
            modelBuilder.Entity<EmployeeProject>()
                .HasOne(ep => ep.Project)
                .WithMany(p => p.EmployeeProjects)
                .HasForeignKey(ep => ep.ProjectId);

            modelBuilder.Entity<WorkActivityHourType>()
                .HasKey(wh => new { wh.WorkActivityId, wh.HourTypeId });
            modelBuilder.Entity<WorkActivityHourType>()
                .HasOne(wh => wh.WorkActivity)
                .WithMany(h => h.WorkActivityHourTypes)
                .HasForeignKey(wh => wh.WorkActivityId);
            modelBuilder.Entity<WorkActivityHourType>()
                .HasOne(wh => wh.HourType)
                .WithMany(h => h.WorkActivityHourTypes)
                .HasForeignKey(wh => wh.HourTypeId);

            modelBuilder.Entity<WorkActivity>()
                .HasOne(x => x.Project)
                .WithMany(x => x.Activities)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.SetNull);
            
            modelBuilder.Entity<TimeLog>()
                .HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);
                
            modelBuilder.Entity<TimeLog>()
                .HasOne(x => x.Activity)
                .WithMany()
                .HasForeignKey(x => x.ActivityId)
                .OnDelete(DeleteBehavior.SetNull);
            
            modelBuilder.Entity<TimeLog>()
                .HasOne(_ => _.HourType)
                .WithMany()
                .HasForeignKey(_ => _.HourTypeId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
