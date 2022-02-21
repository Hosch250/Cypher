namespace Cypher
{
    [AttributeUsage(AttributeTargets.Property)]
    public class RelationshipAttribute : Attribute
    {
        public RelationshipAttribute(string relationship)
        {
            Relationship = relationship;
        }

        public string Relationship { get; }
    }
}