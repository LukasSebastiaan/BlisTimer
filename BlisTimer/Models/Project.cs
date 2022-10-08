namespace BlisTimer.Data
{
    public record Project(string Id, string Name)
    {
        public List<Activity> Activities { get; set; } = null!;
    }
}
