namespace BlisTimer.Models;

public class WorkActivityHourType
{
    public string WorkActivityId { get; set; } 
    public WorkActivity WorkActivity { get; set; }
    public string HourTypeId { get; set; }
    public HourType HourType { get; set; }
}