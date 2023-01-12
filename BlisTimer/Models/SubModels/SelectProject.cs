using Domain.Models;

namespace BlisTimer.Models;

public class SelectProject
{
    public Dictionary<Project, List<WorkActivity>>? ProjectDictionary { get; set; }
    
    public ProjectForm? ProjectForm { get; set; }
}

public class ProjectForm
{
    public string? ProjectId { get; set; }
}