namespace Cypher
{
    public abstract class PersonToMovieRelationship : Node
    {
        public class ACTED_IN : PersonToMovieRelationship
        {
            [Newtonsoft.Json.JsonProperty("roles")]
            public List<string> Roles { get; set; }
        }
        public class DIRECTED : PersonToMovieRelationship { }
        public class WROTE : PersonToMovieRelationship { }
        public class PRODUCED : PersonToMovieRelationship { }
        public class REVIEWED : PersonToMovieRelationship
        {
            [Newtonsoft.Json.JsonProperty("rating")]
            public int Rating { get; set; }

            [Newtonsoft.Json.JsonProperty("summary")]
            public string Summary { get; set; }
        }
    }
}