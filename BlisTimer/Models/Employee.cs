namespace BlisTimer.Models
{
    public class Employee
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public int Role { get; set; }
        public string Password { get; set; } = null!;
        public ICollection<EmployeeProject> EmployeeProjects { get; set; }
        
        public RunningTimer? RunningTimer { get; set; }
        public string? RunningTimerId { get; set; }
    }
}
