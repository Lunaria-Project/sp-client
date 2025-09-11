using System.Collections.Generic;
using Generated;

public partial class GameData
{
    // KeyTestData - KeyTestData, key: Id
    public IReadOnlyDictionary<int, KeyTestData> DTKeyTestData => _dtKeyTestData;
    public bool TryGetKeyTestData(int key, out KeyTestData result) => DTKeyTestData.TryGetValue(key, out result);
    public bool ContainsKeyTestData(int key) => DTKeyTestData.ContainsKey(key);
    private readonly Dictionary<int, KeyTestData> _dtKeyTestData = new();

    // MapData - MapData
    public IReadOnlyList<MapData> DTMapData => _dtMapData;
    private List<MapData> _dtMapData = new();

    // TestData - TestData
    public IReadOnlyList<TestData> DTTestData => _dtTestData;
    private List<TestData> _dtTestData = new();
}
