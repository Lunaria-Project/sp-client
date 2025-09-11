namespace Generated
{
    public partial class ItemData
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string IconResourceKey { get; private set; }

        public ItemData(int id, string name, string iconResourceKey)
        {
            Id = id;
            Name = name;
            IconResourceKey = iconResourceKey;
        }
    }

    public partial class KeyTestData
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public float Hp { get; private set; }
        public ColorType ColorType { get; private set; }

        public KeyTestData(int id, string name, float hp, ColorType colorType)
        {
            Id = id;
            Name = name;
            Hp = hp;
            ColorType = colorType;
        }
    }

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
