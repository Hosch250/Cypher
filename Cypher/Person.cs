namespace Cypher
{
    using static PersonToMovieRelationship;

    [Label("Person")]
    public class Person
    {
        [Key]
        [Newtonsoft.Json.JsonProperty("name")]
        public string Name { get; set; }

        [Newtonsoft.Json.JsonProperty("born")]
        public int? Born { get; set; }

        public virtual IQueryable<(ACTED_IN relationship, Movie movie)> ActedIn { get; }
        public virtual IQueryable<Movie> Directed { get; }
        public virtual IQueryable<Movie> Wrote { get; }
        public virtual IQueryable<Movie> Produced { get; }
        public virtual IQueryable<(REVIEWED relationship, Movie movie)> Reviewed { get; }
    }
}