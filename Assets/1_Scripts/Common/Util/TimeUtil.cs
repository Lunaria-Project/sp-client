using System.Text;
using UnityEngine;

public struct GameTime
{
    public long TotalSeconds { get; private set; }
    public bool IsAM { get; private set; }
    public int HoursForUI { get; private set; }
    public int MinutesForUI { get; private set; }

    public void SetTime(long timeSeconds)
    {
        TotalSeconds = timeSeconds;

        var totalMinutes = TimeUtil.SecondsToMinutes(TotalSeconds);
        var hours = TimeUtil.MinutesToHours(totalMinutes) % TimeUtil.HoursPerDay;

        IsAM = hours < TimeUtil.HoursPerHalfDay;
        HoursForUI = hours switch
        {
            0 => TimeUtil.HoursPerHalfDay,
            < TimeUtil.HoursPerHalfDay => hours,
            TimeUtil.HoursPerHalfDay => TimeUtil.HoursPerHalfDay,
            _ => hours - TimeUtil.HoursPerHalfDay
        };
        MinutesForUI = totalMinutes % TimeUtil.MinutesPerHour;
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
        var hours = SecondsToHours(@this.TotalSeconds);
        var minutes = SecondsToMinutes(@this.TotalSeconds) % MinutesPerHour;
        Sb.AppendFormat(TimeFormat, hours, minutes);
        return Sb.ToString();
    }

    public static string GameTimeToStringForUI(GameTime @this)
    {
        Sb.Clear();
        Sb.AppendFormat(TimeFormat, @this.HoursForUI, @this.MinutesForUI);
        return Sb.ToString();
    }

    public static int GetTenMinuteIntervalIndex(long totalSecondsInDay)
    {
        var totalMinutes = Mathf.FloorToInt((float)totalSecondsInDay / SecondsPerMinute);
        return totalMinutes / MinutesPerInterval;
    }
}