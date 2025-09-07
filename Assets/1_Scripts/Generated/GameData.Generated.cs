using System.Collections.Generic;

namespace Generated
{
    public partial class GameData
    {
        // KeyTestData - KeyTestData, key: Id
        private readonly Dictionary<int, KeyTestData> DTKeyTestData = new();
        public bool TryGetKeyTestData(int key, out KeyTestData result) => DTKeyTestData.TryGetValue(key, out result);
        public bool ContainsKeyTestData(int key) => DTKeyTestData.ContainsKey(key);

        // MapData - MapData
        private readonly List<MapData> DTMapData = new();
        public List<MapData> GetMapDataList => DTMapData;

        // TestData - TestData
        private readonly List<TestData> DTTestData = new();
        public List<TestData> GetTestDataList => DTTestData;

    }
}
