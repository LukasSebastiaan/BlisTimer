namespace BlisTimer.Data
{
    public record Activity(string Id, string Name, string ProjectId)
    {
        public Project Project { get; set; } = null!;
    }
}
