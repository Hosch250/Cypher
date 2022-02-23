namespace Cypher
{
    public enum Direction { From, To}

    [AttributeUsage(AttributeTargets.Property)]
    public class RelationshipAttribute : Attribute
    {
        public RelationshipAttribute(string relationship, Direction direction)
        {
            Relationship = relationship;
            Direction = direction;
        }

        public string Relationship { get; }
        public Direction Direction { get; }
    }
}