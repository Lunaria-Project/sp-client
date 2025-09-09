using System.Collections.Generic;
using Generated;

public partial class GameData
{
    // KeyTestData - KeyTestData
    private readonly List<KeyTestData> DTKeyTestData = new();
    public List<KeyTestData> GetKeyTestDataList => DTKeyTestData;

    // MapData - MapData
    private readonly List<MapData> DTMapData = new();
    public List<MapData> GetMapDataList => DTMapData;

    // TestData - TestData
    private readonly List<TestData> DTTestData = new();
    public List<TestData> GetTestDataList => DTTestData;

}
