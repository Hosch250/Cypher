namespace Cypher
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RelationshipAttribute : Attribute
    {
        public RelationshipAttribute(string relationship, string direction)
        {
            Relationship = relationship;
            Direction = direction;
        }

        public string Relationship { get; }
        public string Direction { get; }
    }
}