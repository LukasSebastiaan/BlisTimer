namespace BlisTimer.Models
{
    public class TimeLog
    {
        public string Id { get; set; } = null!;
        public int AmountOfTimeSpentInSeconds { get; set; }
        public WorkActivity Activity { get; set; } = null!;
        public string ActivityId { get; set; } = null!;
        public Employee Employee { get; set; } = null!;
        public string EmployeeId { get; set; } = null!;
    }
}
