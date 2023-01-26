using System.ComponentModel.DataAnnotations;

namespace Domain.Models;

public class RunningTimer
{
    public string Id { get; set; } = null!;
    public string ActivityId { get; set; }
    public string HourTypeId { get; set; }
    
    [Required]
    public DateTime StartTime { get; set; }
    
    public Employee Employee { get; set; }
    public string EmployeeId { get; set; }
    
}