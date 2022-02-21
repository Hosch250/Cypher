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
    public class Program
    {
        public static async Task Main() {
            Database database = new();
            var person = await database.Persons.Find(f => f.Name == "Tom Cruise");
            var movie = await database.Movies.Find(f => f.Title == "...");

            var persons = await database.Persons.FindAll();
            var movies = await database.Movies.FindAll();

            var personsC = await database.Persons.Count();
            var moviesC = await database.Movies.Count();


            //// runs cypher `match (n:Person { name: "Tom Cruise" }) return n limit 1`
            //var person = database.Persons.Find(a => a.Name == "Tom Cruise")!;

            //// runs cypher `match (:Person { name: "Tom Cruise" })-[r:ACTED_IN]->(m) return r, m`
            //var actedIn = person.ActedIn;

            //// runs cypher `match (:Person { name: "Tom Cruise" })-[:ACTED_IN]->(m:Movie) return m`
            //var movies = person.ActedIn.Select(s => s.movie);

            //// runs cypher `match (:Person { name: "Tom Cruise" })-[r:ACTED_IN]->(:Movie) return r.roles limit 1`
            //var movieRoles = person.ActedIn.First().relationship.Roles;

            //// runs cypher `match (n:Person) return count(n)`
            //var actedInMovieCount = database.Persons.Count();

            // runs provided cypher
            //var unsupportedQuery = await database
            //    .Query<int>("match (:Person)-[r]->(:Movie) return count(r)");
            
            var x = 0;
        }
    }
}