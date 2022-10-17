namespace BlisTimer.Data
{
    public class Activity
    {
        public string Id { get; set; } = null!;
        string Name { get; set; } = null!; 
        string ProjectId { get; set; } = null!;
        public Project Project { get; set; } = null!;
    }
}
