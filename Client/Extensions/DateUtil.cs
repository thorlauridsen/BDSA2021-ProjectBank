namespace ProjectBank.Client.Extensions;

public static class DateExtensions
{
    public static string GetTimeSince(this DateTime date)
    {
        var currentDate = DateTime.Now;
        var diff = currentDate - date;
        if (diff >= TimeSpan.FromDays(365.2425))
        {
            var years = (int) Math.Floor(diff.Days / 365.2425);
            return $"{years} {(years > 1 ? "years" : "year")} ago";
        }
        else if (diff >= TimeSpan.FromDays(30.436875))
        {
            var months = (int) Math.Floor(diff.Days / 30.436875);
            return $"{months} {(months > 1 ? "months" : "month")} ago";
        }
        else if (diff >= TimeSpan.FromDays(1))
        {
            var days = diff.Days;
            return $"{days} {(days > 1 ? "days" : "day")} ago";
        }
        else if (diff >= TimeSpan.FromHours(1))
        {
            var hours = diff.Hours;
            return $"{hours} {(hours > 1 ? "hours" : "hour")} ago";
        }
        else if (diff >= TimeSpan.FromMinutes(1))
        {
            var minutes = diff.Minutes;
            return $"{minutes} {(minutes > 1 ? "minutes" : "minute")} ago";
        }
        else
        {
            return "Now";
        }
    }
}