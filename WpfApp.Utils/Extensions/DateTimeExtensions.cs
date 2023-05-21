namespace WpfApp.Utils.Extensions;

public static class DateTimeExtensions
{
    public static bool EqualTillSeconds(this DateTimeOffset firstDateTime, DateTimeOffset secondDateTime)
    {
        return firstDateTime.ToString("G") == secondDateTime.ToString("G");
    }
    
    public static string ToStringTillSeconds(this DateTimeOffset dateTime)
    {
        return dateTime.ToString("G");
    }
}