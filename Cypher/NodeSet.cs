namespace Cypher
{
    using Neo4jClient.Cypher;
    using System.Linq.Expressions;

    public class NodeSet<T> where T : Node
    {
        public async ValueTask<T?> Find(Expression<Func<T, bool>> expression)
        {
            var client = new Neo4jClient.GraphClient(new Uri("http://localhost:7474"));
            await client.ConnectAsync();

            var label = typeof(T).GetCustomAttributes(typeof(LabelAttribute), false).FirstOrDefault() is LabelAttribute labelAttr
                ? labelAttr.Name
                : typeof(T).Name;

            var nodeName = expression?.Parameters[0].Name ?? "data";
            var query = client.Cypher.Match($"({nodeName}:{label})");

            if (expression is not null)
            {
                query = query.Where(expression);
            }

            var queryReturn = query.Return(() => new
                {
                    data = Return.As<T>(nodeName),
                    id = Return.As<long>($"id({nodeName})")
                })
                .Limit(1);

            return (await queryReturn.ResultsAsync).Select(s => {
                s.data.Identity = s.id;
                return s.data;
            }).FirstOrDefault();
        }

        public async ValueTask<IReadOnlyList<T>> FindAll(Expression<Func<T, bool>>? expression = null)
        {
            var client = new Neo4jClient.GraphClient(new Uri("http://localhost:7474"));
            await client.ConnectAsync();

            var label = typeof(T).GetCustomAttributes(typeof(LabelAttribute), false).FirstOrDefault() is LabelAttribute labelAttr
                ? labelAttr.Name
                : typeof(T).Name;

            var nodeName = expression?.Parameters[0].Name ?? "data";
            var query = client.Cypher.Match($"({nodeName}:{label})");

            if (expression is not null)
            {
                query = query.Where(expression);
            }
            var queryReturn = query.Return((data) => new
                {
                    data = Return.As<T>(nodeName),
                    id = Return.As<long>($"id({nodeName})")
                });

            return (await queryReturn.ResultsAsync).Select(s => {
                s.data.Identity = s.id;
                return s.data;
            }).ToList()!;
        }

        public async ValueTask<long> Count(Expression<Func<T, bool>>? expression = null)
        {
            var client = new Neo4jClient.GraphClient(new Uri("http://localhost:7474"));
            await client.ConnectAsync();

            var label = typeof(T).GetCustomAttributes(typeof(LabelAttribute), false).FirstOrDefault() is LabelAttribute labelAttr
                ? labelAttr.Name
                : typeof(T).Name;

            var nodeName = expression?.Parameters[0].Name ?? "data";
            var query = client.Cypher.Match($"({nodeName}:{label})");

            if (expression is not null)
            {
                query = query.Where(expression);
            }

            var queryReturn = query.Return((data) => new
                {
                    data = Return.As<int>($"count({nodeName})")
                });

            return (await queryReturn.ResultsAsync).First().data;
        }
    }
}