using System.ComponentModel.DataAnnotations;
using BlisTimer.Controllers;

namespace BlisTimer.Models
{
    public class TimeLog
    {
        public string Id { get; set; } = null!;

        [Required]
        public DateTime StartTime { get; set; }
        [Required]
        public DateTime EndTime { get; set; }
        
        public WorkActivity Activity { get; set; } = null!;
        public string ActivityId { get; set; } = null!;
        
        public Employee Employee { get; set; } = null!;
        public string EmployeeId { get; set; } = null!;

        public HourType HourType { get; set; } = null!;
        public string HourTypeId { get; set; } = null!;
    }
}
