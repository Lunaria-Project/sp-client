using System.Collections.Generic;
using Generated;

public partial class GameData
{
    // ItemData - ItemData, key: Id
    public IReadOnlyDictionary<int, ItemData> DTItemData => _dtItemData;
    public bool TryGetItemData(int key, out ItemData result) => DTItemData.TryGetValue(key, out result);
    public bool ContainsItemData(int key) => DTItemData.ContainsKey(key);
    private readonly Dictionary<int, ItemData> _dtItemData = new();
}
