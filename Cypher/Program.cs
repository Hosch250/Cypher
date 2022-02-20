// official Neo4j client
//using Neo4j.Driver;

//using var driver = GraphDatabase.Driver("neo4j://localhost:7687");
//using var session = driver.AsyncSession();

//var query = new Query("match (n:Person {name: 'Tom Hanks'})<-[r]->(m:Movie) return n{.*, movies: collect(m{.*, relationship: Type(r), roles: Properties(r).roles})}");
//var result = await session.RunAsync(query);

//while (await result.FetchAsync())
//{
//    var record = result.Current;
//}


// official Neo4j client with reactive extensions
//using Neo4j.Driver;
//using System.Reactive.Linq;

//using var driver = GraphDatabase.Driver("neo4j://localhost:7687");
//var session = driver.RxSession();

//var query = new Query("match (n:Person {name: 'Tom Hanks'})<-[r]->(m:Movie) return n{.*, movies: collect(m{.*, relationship: Type(r), roles: Properties(r).roles})}");
//var result = session.Run(query);

//var records = result.Records();

//foreach (var record in records)
//{
//    var p = record["n"];
//}


// community client
//using Neo4jClient.Cypher;

//var client = new Neo4jClient.GraphClient(new Uri("http://localhost:7474"));
//await client.ConnectAsync();

//var query = client.Cypher
//    .Match("(n: Person { name: 'Tom Hanks'})<-[r]->(m: Movie)")
//    //.Return<Person>("n{.*, movies: collect(m{.*, relationship: Type(r), roles: Properties(r).roles})}");
//    .Return((n, r, m) => new
//     {
//         person = Return.As<Person>("n{.*, movies: collect(m{.*, relationship: Type(r), roles: Properties(r).roles})}")
//     });

//var data = (await query.ResultsAsync).ToList();

//record Person
//{
//    [Newtonsoft.Json.JsonProperty("born")]
//    public int Born { get; set; }

//    [Newtonsoft.Json.JsonProperty("name")]
//    public string Name { get; set; }

//    [Newtonsoft.Json.JsonProperty("movies")]
//    public List<Movie> Movies { get; set; }
//}
//record Movie
//{
//    [Newtonsoft.Json.JsonProperty("released")]
//    public int Released { get; set; }

//    [Newtonsoft.Json.JsonProperty("tagline")]
//    public string Tagline { get; set; }

//    [Newtonsoft.Json.JsonProperty("title")]
//    public string Title { get; set; }

//    [Newtonsoft.Json.JsonProperty("relationship")]
//    public Relationship Relationship { get; set; }

//    [Newtonsoft.Json.JsonProperty("roles")]
//    public List<string>? Roles { get; set; }
//}
//enum Relationship
//{
//    ACTED_IN,
//    DIRECTED
//}

namespace Cypher
{
    using System.Linq.Expressions;
    using static PersonToMovieRelationship;

    [AttributeUsage(AttributeTargets.Class)]
    public class LabelAttribute : Attribute
    {
        public LabelAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class RelationshipAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public class KeyAttribute : Attribute { }

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

    [Label("Movie")]
    public class Movie
    {
        [Key]
        public string Title { get; set; }
        public string Tagline { get; set; }
        public int Released { get; set; }

        public IQueryable<(ACTED_IN relationship, Person person)> Actors { get; }  // we want to know the role the actor had
        public IQueryable<Person> DirectedBy { get; }                 // this relationship doesn't have roles, we just want the node
        public IQueryable<Person> WrittenBy { get; }
        public IQueryable<Person> ProducedBy { get; }
        public IQueryable<(REVIEWED relationship, Person person)> ReviewedBy { get; }
    }

    [Label("Person")]
    public class Person
    {
        [Key]
        public string Name { get; set; }
        public int? Born { get; set; }

        public IQueryable<(ACTED_IN relationship, Movie movie)> ActedIn { get; }
        public IQueryable<Movie> Directed { get; }
        public IQueryable<Movie> Wrote { get; }
        public IQueryable<Movie> Produced { get; }
        public IQueryable<(REVIEWED relationship, Movie movie)> Reviewed { get; }
    }

    public abstract class IDatabase {
        public T? Query<T>(string cypher) => default;
    }
    public abstract class INodeSet<T>
    {
        // this will perform the following actions:
        // 1. find node label; this will be `typeof(T)`, find `Label` attribute, use `Name` property: OR use type name if no `Label` attribute
        // 2. evaluate expression into json match; todo: create/apply attribute to override property name
        // 3. sanitize match expression, if necessary
        // 4. build and run query expression
        // 5. deserialize and return response
        public T? Find(Expression<Func<T, bool>> expression) => default;

        // this will perform the following actions:
        // 1. if an expression is provided
        //    1. find node label; this will be `typeof(T)`, find `Label` attribute, use `Name` property: OR use type name if no `Label` attribute
        //    2. evaluate expression into json match; todo: create/apply attribute to override property name
        //    3. sanitize match expression, if necessary
        // 2. build and run query expression
        // 3. deserialize and return response
        public IReadOnlyList<T> FindAll(Expression<Func<T, bool>>? expression = null) => new List<T>();

        // this will perform the following actions:
        // 1. if an expression is provided
        //    1. find node label; this will be `typeof(T)`, find `Label` attribute, use `Name` property: OR use type name if no `Label` attribute
        //    2. evaluate expression into json match; todo: create/apply attribute to override property name
        //    3. sanitize match expression, if necessary
        // 2. build and run query expression
        // 3. deserialize and return response
        public int Count(Expression<Func<T, bool>>? expression = null) => 0;
    }

    public class Database : IDatabase
    {
        public INodeSet<Person> Persons { get; }
        public INodeSet<Movie> Movies { get; }
    }

    public class Entry
    {
        public void Main() {
            Database database = null!;

            // runs cypher `match (n:Person { name: "Tom Cruise" }) return n limit 1`
            var person = database.Persons.Find(a => a.Name == "Tom Cruise")!;

            // runs cypher `match (:Person { name: "Tom Cruise" })-[r:ACTED_IN]->(m) return r, m`
            var actedIn = person.ActedIn;

            // runs cypher `match (:Person { name: "Tom Cruise" })-[:ACTED_IN]->(m:Movie) return m`
            var movies = person.ActedIn.Select(s => s.movie);

            // runs cypher `match (:Person { name: "Tom Cruise" })-[r:ACTED_IN]->(:Movie) return r.roles limit 1`
            var movieRoles = person.ActedIn.First().relationship.Roles;

            // runs cypher `match (n:Person) return count(n)`
            var actedInMovieCount = database.Persons.Count();

            // runs provided cypher
            var unsupportedQuery = database
                .Query<int>("match (:Person)-[r]->(:Movie) return count(r)");
        }
    }
}