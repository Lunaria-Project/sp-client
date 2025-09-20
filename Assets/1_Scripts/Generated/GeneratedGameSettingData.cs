using System;

public class GameSetting : Singleton<GameSetting>
{
    public int SecondsPerGameHour { get; private set; }
    public int StartUserHp { get; private set; }
    public int MaximumUserHp { get; private set; }
    public int LunitesPerSolari { get; private set; }
    public int GrainsPerLunite { get; private set; }
    public int StartUserLunite { get; private set; }
    public int MaximumSolariCount { get; private set; }

    public void InvokeLoadForSheet(SheetInfo sheetInfo)
    {
        foreach (var row in sheetInfo.Rows)
        {
            var name = (string)row[1];
            switch (name)
            {
                case "SecondsPerGameHour": SecondsPerGameHour = Convert.ToInt32(row[2]); break;
                case "StartUserHp": StartUserHp = Convert.ToInt32(row[2]); break;
                case "MaximumUserHp": MaximumUserHp = Convert.ToInt32(row[2]); break;
                case "LunitesPerSolari": LunitesPerSolari = Convert.ToInt32(row[2]); break;
                case "GrainsPerLunite": GrainsPerLunite = Convert.ToInt32(row[2]); break;
                case "StartUserLunite": StartUserLunite = Convert.ToInt32(row[2]); break;
                case "MaximumSolariCount": MaximumSolariCount = Convert.ToInt32(row[2]); break;
            }
        }
    }
}
