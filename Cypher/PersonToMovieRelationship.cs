namespace Cypher
{
    [Relationship]
    public abstract record PersonToMovieRelationship
    {
        public record ACTED_IN(IReadOnlyList<string> Roles) : PersonToMovieRelationship { }
        public record DIRECTED : PersonToMovieRelationship { }
        public record WROTE : PersonToMovieRelationship { }
        public record PRODUCED : PersonToMovieRelationship { }
        public record REVIEWED(int Rating, string Summary) : PersonToMovieRelationship { }

        public static ACTED_IN ActedIn(IReadOnlyList<string> Roles) => new(Roles);
        public static DIRECTED Directed => new();
        public static WROTE Wrote => new();
        public static PRODUCED Produced => new();
        public static REVIEWED Reviewed(int Rating, string Summary) => new(Rating, Summary);
    }
}