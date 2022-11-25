using BlisTimer.Models;
using Microsoft.EntityFrameworkCore;

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
        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseSerialColumns();
            
            modelBuilder.Entity<HourType>()
                .HasOne(_ => _.WorkActivity)
                .WithMany(_ => _.HourTypes)
                .HasForeignKey(_ => _.WorkActivityId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Employee>()
                .HasMany(x => x.Projects)
                .WithMany(x => x.Employees);
                
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
