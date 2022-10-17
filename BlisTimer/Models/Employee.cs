namespace BlisTimer.Data
{
    public class Employee
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public int Role;
        public List<Project> Projects { get; set; } = null!;
    }
}
