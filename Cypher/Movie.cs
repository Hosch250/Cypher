namespace Cypher
{
    using static PersonToMovieRelationship;

    [Label("Movie")]
    public class Movie : Node
    {
        [Newtonsoft.Json.JsonProperty("title")]
        public string Title { get; set; }

        [Newtonsoft.Json.JsonProperty("tagline")]
        public string Tagline { get; set; }

        [Newtonsoft.Json.JsonProperty("released")]
        public int Released { get; set; }

        [Relationship(nameof(ACTED_IN), "TO")]
        public virtual IEnumerable<(ACTED_IN relationship, Person person)> Actors => this.GetNodesWithRelationship<ACTED_IN, Person>();

        [Relationship(nameof(DIRECTED), "TO")]
        public virtual IEnumerable<Person> DirectedBy => this.GetNodesByRelationship<Person>();

        [Relationship(nameof(WROTE), "TO")]
        public virtual IEnumerable<Person> WrittenBy => this.GetNodesByRelationship<Person>();

        [Relationship(nameof(PRODUCED), "TO")]
        public virtual IEnumerable<Person> ProducedBy => this.GetNodesByRelationship<Person>();

        [Relationship(nameof(REVIEWED), "TO")]
        public virtual IEnumerable<(REVIEWED relationship, Person person)> ReviewedBy => this.GetNodesWithRelationship<REVIEWED, Person>();
    }
}