﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

public class Preferences
{
    public string Id { get; set; }
    
    public int NotificationEnabled { get; set; }
    public int NotificationTimeSeconds { get; set; }
    public int ChangeCountTimeSeconds { get; set; }
    
    // TODO: Add more preference variables here
    
    [ForeignKey("EmployeeId")]
    public Employee Employee { get; set; }
    public string EmployeeId { get; set; }
}