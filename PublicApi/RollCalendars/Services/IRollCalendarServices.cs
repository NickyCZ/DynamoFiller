namespace PublicApi.RollCalendars.Services;

public interface IRollCalendarServices
{
    Task AddRollCalendar(List<string> instrument);
}
