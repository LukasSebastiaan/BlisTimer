namespace Domain.Models;

public class Employee
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public ICollection<EmployeeProject> EmployeeProjects { get; set; }
}
