﻿
namespace Domain.Models;

public class WorkActivity
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!; 
    public string ProjectId { get; set; } = null!;
    public Project Project { get; set; } = null!;

    public ICollection<WorkActivityHourType> WorkActivityHourTypes { get; set; }
}
