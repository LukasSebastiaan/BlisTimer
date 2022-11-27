namespace BlisTimer.Models;

public class RunningTimers
{
    public string Id { get; set; } = null!;
    public string ActivityId { get; set; }
    public string HourTypeId { get; set; }
    public DateTime StartTime { get; set; }
    
    public Employee Employee { get; set; }
    public string EmployeeId { get; set; }
    
}