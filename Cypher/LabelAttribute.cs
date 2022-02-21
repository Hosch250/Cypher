namespace Cypher
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LabelAttribute : Attribute
    {
        public LabelAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}