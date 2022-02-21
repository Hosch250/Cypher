namespace Cypher
{
    using static PersonToMovieRelationship;

    [Label("Person")]
    public class Person : Node
    {
        [Newtonsoft.Json.JsonProperty("name")]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonProperty("born")]
        public int? Born { get; set; }

        [Relationship(nameof(ACTED_IN), "FROM")]
        public virtual IReadOnlyList<(ACTED_IN relationship, Movie movie)> ActedIn => this.GetNodesWithRelationship<ACTED_IN, Movie>();

        [Relationship(nameof(DIRECTED), "FROM")]
        public virtual IReadOnlyList<Movie> Directed => this.GetNodesByRelationship<Movie>();

        [Relationship(nameof(WROTE), "FROM")]
        public virtual IReadOnlyList<Movie> Wrote => this.GetNodesByRelationship<Movie>();

        [Relationship(nameof(PRODUCED), "FROM")]
        public virtual IReadOnlyList<Movie> Produced => this.GetNodesByRelationship<Movie>();

        [Relationship(nameof(REVIEWED), "FROM")]
        public virtual IReadOnlyList<(REVIEWED relationship, Movie movie)> Reviewed => this.GetNodesWithRelationship<REVIEWED, Movie>();
    }
}