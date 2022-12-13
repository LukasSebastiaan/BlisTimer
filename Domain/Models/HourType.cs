namespace Domain.Models;

public class HourType
{
    public string HourTypeId { get; set; } = null!;
    public string Label { get; set; } = null!;
    public string Type { get; set; } = null!;
    
    public ICollection<WorkActivityHourType> WorkActivityHourTypes { get; set; }
}