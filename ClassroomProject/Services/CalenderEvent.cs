namespace ClassroomProject.Services;

public class CalendarEvent
{
    public string title { get; set; } = "";
    public string start { get; set; } = "";
    public string? end { get; set; }
    public string? color { get; set; }
    public bool allDay { get; set; } = false;
    
    public object? rrule { get; set; }

}
// public class RRuleData
// {
//     public string freq { get; set; } = "weekly";
//     public string[] byweekday { get; set; } = Array.Empty<string>();
//     public string dtstart { get; set; } = "";
//     public string until { get; set; } = "";
// }