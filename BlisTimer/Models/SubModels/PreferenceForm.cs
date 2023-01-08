using System.ComponentModel.DataAnnotations.Schema;

namespace BlisTimer.Models;

public class PreferencesForm
{
    public bool NotificationEnabled { get; set; }
    public int NotificationTimeHours { get; set; }
    // TODO: Add more preference variables here
}