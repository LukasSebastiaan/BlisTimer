namespace BlisTimer.Models;

public class PreferencesForm
{
    public bool NotificationEnabled { get; set; }
    public int NotificationTimeHours { get; set; }
    public int ChangeCountTimeSeconds { get; set; }
}