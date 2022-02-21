namespace Cypher
{
    using static PersonToMovieRelationship;

    [Label("Movie")]
    public class Movie
    {
        [Key]
        [Newtonsoft.Json.JsonProperty("title")]
        public string Title { get; set; }

        [Newtonsoft.Json.JsonProperty("tagline")]
        public string Tagline { get; set; }

        [Newtonsoft.Json.JsonProperty("released")]
        public int Released { get; set; }

        public virtual IQueryable<(ACTED_IN relationship, Person person)> Actors { get; }   // we want to know the role the actor had
        public virtual IQueryable<Person> DirectedBy { get; }                               // this relationship doesn't have roles, we just want the node
        public virtual IQueryable<Person> WrittenBy { get; }
        public virtual IQueryable<Person> ProducedBy { get; }
        public virtual IQueryable<(REVIEWED relationship, Person person)> ReviewedBy { get; }
    }
}