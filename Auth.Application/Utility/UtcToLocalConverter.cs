namespace Auth.Application.Utility;

public static class UtcToLocalConverter
{
public static string ConvertToLocalTime(DateTime utcDateTime)
{
    TimeZoneInfo localZone = TimeZoneInfo.Local;
    DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, localZone);
    return localDateTime.ToString("yyyy-MM-ddTHH:mm:ss");
}
}