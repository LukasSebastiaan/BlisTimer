namespace Domain.Models;

public class EmployeeProject
{
    public string EmployeeId { get; set; } 
    public Employee Employee { get; set; }
    public string ProjectId { get; set; }
    public Project Project { get; set; }
}