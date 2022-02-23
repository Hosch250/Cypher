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

        [Relationship(nameof(ACTED_IN), Direction.From)]
        public virtual IReadOnlyList<(ACTED_IN relationship, Movie movie)> ActedIn => this.GetNodesWithRelationship<ACTED_IN, Movie>();

        [Relationship(nameof(DIRECTED), Direction.From)]
        public virtual IReadOnlyList<Movie> Directed => this.GetNodesByRelationship<Movie>();

        [Relationship(nameof(WROTE), Direction.From)]
        public virtual IReadOnlyList<Movie> Wrote => this.GetNodesByRelationship<Movie>();

        [Relationship(nameof(PRODUCED), Direction.From)]
        public virtual IReadOnlyList<Movie> Produced => this.GetNodesByRelationship<Movie>();

        [Relationship(nameof(REVIEWED), Direction.From)]
        public virtual IReadOnlyList<(REVIEWED relationship, Movie movie)> Reviewed => this.GetNodesWithRelationship<REVIEWED, Movie>();
    }
}