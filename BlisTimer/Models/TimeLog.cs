namespace BlisTimer.Data
{
    public class TimeLog
    {
        public string Id { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string ProjectId { get; set; } = null!;
        public string ActivityId { get; set; } = null!;
        public int AmountOfTimeSpentInSeconds { get; set; }
        public Project Project { get; set; }
        public Activity Activity { get; set; }
    }
}
