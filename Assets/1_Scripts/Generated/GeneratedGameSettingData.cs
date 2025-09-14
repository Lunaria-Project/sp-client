using System;

public class GameSetting : Singleton<GameSetting>
{
    public int SecondsPerGameHour { get; private set; }

    public void InvokeLoadForSheet(SheetInfo sheetInfo)
    {
        foreach (var row in sheetInfo.Rows)
        {
            var name = (string)row[1];
            switch (name)
            {
                case "SecondsPerGameHour": SecondsPerGameHour = Convert.ToInt32(row[2]); break;
            }
        }
    }
}
