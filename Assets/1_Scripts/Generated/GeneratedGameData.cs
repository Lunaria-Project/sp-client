namespace Generated
{
    public partial class MapData
    {
        public string MapResourceKey { get; private set; }

        public MapData(string mapResourceKey)
        {
            MapResourceKey = mapResourceKey;
        }
    }

    public partial class TestData
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public float Hp { get; private set; }
        public ColorType ColorType { get; private set; }

        public TestData(int id, string name, float hp, ColorType colorType)
        {
            Id = id;
            Name = name;
            Hp = hp;
            ColorType = colorType;
        }
    }

}
