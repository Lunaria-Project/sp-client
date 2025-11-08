using System;
using System.Collections.Generic;
using Generated;

public partial class GameData
{
    private void LoadItemData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        foreach (var row in rows)
        {
            var newData = new ItemData(Convert.ToInt32(row[0]), (row[1] as string) ?? string.Empty, (row[2] as string) ?? string.Empty);
            _dtItemData.Add(newData.Id, newData);
        }
    }

    private void InvokeLoadForSheet(string sheetName, List<object[]> rows)
    {
        switch (sheetName)
        {
            case "ItemData": LoadItemData(rows); break;
        }
    }
}
