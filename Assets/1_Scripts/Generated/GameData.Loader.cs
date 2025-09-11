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

    private void LoadKeyTestData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        foreach (var row in rows)
        {
            var newData = new KeyTestData(Convert.ToInt32(row[0]), (row[1] as string) ?? string.Empty, Convert.ToSingle(row[2]), (ColorType)Enum.Parse(typeof(ColorType), (string)row[3], true));
            _dtKeyTestData.Add(newData.Id, newData);
        }
    }

    private void LoadMapData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        foreach (var row in rows)
        {
            var newData = new MapData((row[0] as string) ?? string.Empty);
            _dtMapData.Add(newData);
        }
    }

    private void LoadTestData(List<object[]> rows)
    {
        if (rows.IsNullOrEmpty()) return;
        foreach (var row in rows)
        {
            var newData = new TestData(Convert.ToInt32(row[0]), (row[1] as string) ?? string.Empty, Convert.ToSingle(row[2]), (ColorType)Enum.Parse(typeof(ColorType), (string)row[3], true));
            _dtTestData.Add(newData);
        }
    }

    private void InvokeLoadForSheet(string sheetName, List<object[]> rows)
    {
        switch (sheetName)
        {
            case "ItemData": LoadItemData(rows); break;
            case "KeyTestData": LoadKeyTestData(rows); break;
            case "MapData": LoadMapData(rows); break;
            case "TestData": LoadTestData(rows); break;
        }
    }
}
