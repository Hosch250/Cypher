namespace Cypher
{
    public class Database : IDatabase
    {
        public NodeSet<Person> Persons { get; } = new();
        public NodeSet<Movie> Movies { get; } = new();
    }
}