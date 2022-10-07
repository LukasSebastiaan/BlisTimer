namespace BlisTimer.Data
{
    public class TimeLog
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ProjectId { get; set; }
        public string ActivityId { get; set; }
        public int AmountOfTimeSpentInSeconds { get; set; }
    }
}
