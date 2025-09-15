using System;
using UnityEngine;

public partial class GameData : Singleton<GameData>
{
    public void LoadGameData()
    {
#if UNITY_EDITOR
        var sheets = JsonDataLoader.LoadAllSheets();
#else
        var sheets = new List<SheetInfo>();
        LogManager.LogError("빌드했을 때 데이터 로드를 지원하지 않음");
        return;
#endif
        if (sheets.IsNullOrEmpty())
        {
            LogManager.LogWarning("[GameDataRuntimeLoader] No sheets found.");
            return;
        }
        
        const string EnumDataFileName = "EnumData";
        const string GameSettingDataFileName = "GameSettingData";

        foreach (var sheetInfo in sheets)
        {
            var isEnumData = string.Equals(sheetInfo.SheetName, EnumDataFileName, StringComparison.OrdinalIgnoreCase);
            if (isEnumData) continue;
            var isGameSettingData = string.Equals(sheetInfo.SheetName, GameSettingDataFileName, StringComparison.OrdinalIgnoreCase);
            if (isGameSettingData)
            {
                GameSetting.Instance.InvokeLoadForSheet(sheetInfo);
                continue;
            }
            InvokeLoadForSheet(sheetInfo.SheetName, sheetInfo.Rows);
        }
    }
}