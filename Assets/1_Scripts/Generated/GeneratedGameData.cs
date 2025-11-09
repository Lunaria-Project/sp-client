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

    public partial class RequirementInfoData
    {
        public string RequirementType { get; private set; }

        public RequirementInfoData(string requirementType)
        {
            RequirementType = requirementType;
        }
    }
}
