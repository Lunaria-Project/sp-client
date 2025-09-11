using UnityEngine;

public partial class GameData : Singleton<GameData>
{ 
    public void LoadGameData()
    {
#if UNITY_EDITOR
        var sheets = JsonDataLoader.LoadAllSheets();
#else
        var sheets = new List<SheetInfo>();
        Debug.LogError("빌드했을 때 데이터 로드를 지원하지 않음");
        return;
#endif
        if (sheets.IsNullOrEmpty())
        {
            Debug.LogWarning("[GameDataRuntimeLoader] No sheets found.");
            return;
        }

        foreach (var sheetInfo in sheets)
        {
            InvokeLoadForSheet(sheetInfo.SheetName, sheetInfo.Rows);
        }
    }
}
