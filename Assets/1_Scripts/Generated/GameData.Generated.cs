using System.Collections.Generic;

namespace Generated
{
    public partial class GameData
    {
        private readonly List<MapData> DTMapData = new();
        public List<MapData> GetMapDataList => DTMapData;

        private readonly List<TestData> DTTestData = new();
        public List<TestData> GetTestDataList => DTTestData;

    }
}
