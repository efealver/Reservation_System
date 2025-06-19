using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;

public class HolidayChecker
{
    private readonly CalendarService _calendarService;
    private const string CalendarId = "en.turkish#holiday@group.v.calendar.google.com";

    public HolidayChecker()
    {
        GoogleCredential credential = GoogleCredential.FromFile("credentials.json")
            .CreateScoped(CalendarService.Scope.CalendarReadonly);

        _calendarService = new CalendarService(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = "ClassroomProject",
        });
    }

    public async Task<bool> IsHolidayAsync(DateTime date)
    {
        var request = _calendarService.Events.List(CalendarId);
        request.TimeMin = date.Date;
        request.TimeMax = date.Date.AddDays(1);
        request.SingleEvents = true;

        Events events = await request.ExecuteAsync();
        return events.Items != null && events.Items.Any();
    }
}
