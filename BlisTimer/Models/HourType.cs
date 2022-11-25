namespace BlisTimer.Models;

public class HourType
{
    public string HourTypeId { get; set; } = null!;
    public string Label { get; set; } = null!;
    public string Type { get; set; } = null!;
    
    public WorkActivity WorkActivity { get; set; } = null!;
    public string WorkActivityId { get; set; } = null!;

}