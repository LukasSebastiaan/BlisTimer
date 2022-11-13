namespace BlisTimer.Models
{
    public class Project
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public List<Activity> Activities { get; set; } = null!;
        public List<Employee> Employees { get; set; } = null!;
    }
}
