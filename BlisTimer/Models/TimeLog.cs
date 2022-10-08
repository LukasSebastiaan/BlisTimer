namespace BlisTimer.Data
{
    public record TimeLog(string Id, string UserId, string ProjectId, string ActivityId, int AmountOfTimeSpentInSeconds)
    {
        public Project Project { get; set; } = null!;
        public Activity Activity { get; set; } = null!;
    }
}
