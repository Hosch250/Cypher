namespace Cypher
{
    using Neo4jClient.Cypher;
    using System.Runtime.CompilerServices;
    using static PersonToMovieRelationship;

    [Label("Person")]
    public class Person : Node
    {
        [Key]
        [Newtonsoft.Json.JsonProperty("name")]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonProperty("born")]
        public int? Born { get; set; }

        [Relationship(nameof(ACTED_IN))]
        public virtual IReadOnlyList<(ACTED_IN relationship, Movie movie)> ActedIn { get; }

        [Relationship(nameof(DIRECTED))]
        public virtual IReadOnlyList<Movie> Directed => this.GetNodesByRelationship<Person, Movie>();

        [Relationship(nameof(WROTE))]
        public virtual IReadOnlyList<Movie> Wrote => this.GetNodesByRelationship<Person, Movie>();

        [Relationship(nameof(PRODUCED))]
        public virtual IReadOnlyList<Movie> Produced => this.GetNodesByRelationship<Person, Movie>();

        [Relationship(nameof(REVIEWED))]
        public virtual IReadOnlyList<(REVIEWED relationship, Movie movie)> Reviewed { get; }
    }

    public static class NodeExtensions
    {
        public static IReadOnlyList<TResponse> GetNodesByRelationship<T, TResponse>(this object node, [CallerMemberName] string callerMemberName = "")
        {
            var client = new Neo4jClient.GraphClient(new Uri("http://localhost:7474"));
            client.ConnectAsync().Wait();

            var fromLabel = typeof(T).GetCustomAttributes(typeof(LabelAttribute), false).FirstOrDefault() is LabelAttribute labelAttr
                ? labelAttr.Name
                : typeof(T).Name;

            var toLabel = typeof(TResponse).GetCustomAttributes(typeof(LabelAttribute), false).FirstOrDefault() is LabelAttribute labelAttr1
                ? labelAttr1.Name
                : typeof(TResponse).Name;

            var relationship = typeof(T).GetProperty(callerMemberName)!.GetCustomAttributes(typeof(RelationshipAttribute), false).FirstOrDefault() is RelationshipAttribute relAttr
                ? relAttr.Relationship
                : callerMemberName;

            var keyProp = typeof(T).GetProperties().First(f => f.CustomAttributes.Any(a => a.AttributeType == typeof(KeyAttribute)));
            var keyName = keyProp.GetCustomAttributes(typeof(Newtonsoft.Json.JsonPropertyAttribute), false).FirstOrDefault() is Newtonsoft.Json.JsonPropertyAttribute jpa
                ? jpa.PropertyName
                : keyProp.Name;

            var propValue = keyProp.GetValue(node);
            var keyValue = propValue is string ? $"\"{propValue}\"" : propValue;

            var key = $"{{ {keyName}: {keyValue} }}";

            var query = client.Cypher
                .Match($"(:{fromLabel} {key})-[:{relationship}]->(data:{toLabel})")
                .Return((data) => data.As<TResponse>());

            return query.ResultsAsync.Result.ToList()!;
        }
    }
}