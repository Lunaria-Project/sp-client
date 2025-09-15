using System.Text;

public struct GameTime
{
    public int HoursForUI { get; private set; }
    public int Hours { get; private set; }
    public int Minutes { get; private set; }
    public long TotalSeconds { get; private set; }
    public bool IsAM { get; private set; }

    public void SetTime(long timeSeconds)
    {
        Minutes = TimeUtil.SecondsToMinutes(timeSeconds);
        Hours = TimeUtil.MinutesToHours(Minutes);
        HoursForUI = Hours;
        TotalSeconds = timeSeconds;
        IsAM = true;

        if (Hours <= TimeUtil.HoursPerHalfDay) return;
        
        HoursForUI -= TimeUtil.HoursPerHalfDay;
        IsAM = false;
    }
}

public static class TimeUtil
{
    public const int SecondsPerDay = HoursPerDay * MinutesPerHour * SecondsPerMinute;
    public const int SecondsPerMinute = 60;
    public const int MinutesPerHour = 60;
    public const int HoursPerDay = 24;
    public const int HoursPerHalfDay = 12;
    public const int MinutesPerInterval = 10;
    private const string TimeFormat = "{0:00}:{1:00}";
    private static readonly StringBuilder Sb = new(8);

    public static int SecondsToMinutes(float timeSeconds) => (int)(timeSeconds / SecondsPerMinute);
    public static int MinutesToHours(float timeMinutes) => (int)(timeMinutes / MinutesPerHour);
    public static int SecondsToHours(float timeSeconds) => MinutesToHours(SecondsToMinutes(timeSeconds));

    public static GameTime GetGameTime(long timeSeconds)
    {
        var gameTime = new GameTime();
        gameTime.SetTime(timeSeconds);
        return gameTime;
    }

    public static string GameTimeToString(GameTime @this)
    {
        Sb.Clear();
        Sb.AppendFormat(TimeFormat, @this.Hours, @this.Minutes);
        return Sb.ToString();
    }
}