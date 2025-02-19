using SQLite;

namespace FestivalPlannerApp.Models
{
    public class Day
    {
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public bool AfterMidnight { get; set; }
        public DateTime MinStartDateTime { get; set; } = DateTime.Today;
        public DateTime MaxStartDateTime { get; set; }
        public DateTime MinEndDateTime { get; set; }
        public DateTime MaxEndDateTime { get; set; }
        
    }
}
