namespace Domain.Models
{
    public class Project
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public List<WorkActivity>? Activities { get; set; } = new List<WorkActivity>();
        
        public ICollection<EmployeeProject> EmployeeProjects { get; set; }
    }
}
