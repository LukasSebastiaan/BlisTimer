namespace BlisTimer.Data
{
    public class Project
    {
        public string Id { get; set; } = null!;
        string Name { get; set; } = null!;
        public List<Activity> Activities { get; set; } = null!;

    }
}
