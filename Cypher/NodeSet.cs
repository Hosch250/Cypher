namespace Cypher
{
    using Neo4jClient.Cypher;
    using System.Linq.Expressions;
    using System.Text;

    public static class ExpressionParser
    {
        public static string Parse<T>(Expression<Func<T, bool>> expression)
        {
            var builder = new StringBuilder();
            builder.Append('{');

            Listener(builder, (dynamic)expression.Body);

            builder.Append('}');

            return builder.ToString();
        }

        public static void Listener(StringBuilder builder, BinaryExpression expression)
        {
            Listener(builder, (dynamic)expression.Left);
            Listener(builder, (dynamic)expression.Right);
        }

        public static void Listener(StringBuilder builder, ConstantExpression expression)
        {
            var dict = new Dictionary<Type, Action>
            {
                [typeof(string)] = () => builder.Append($"\"{expression.Value}\""),
                [typeof(int)] = () => builder.Append(expression.Value),
            };

            dict[expression.Type]();
        }

        public static void Listener(StringBuilder builder, MemberExpression expression)
        {
            var name = expression.Member.GetCustomAttributes(typeof(Newtonsoft.Json.JsonPropertyAttribute), false).FirstOrDefault() is Newtonsoft.Json.JsonPropertyAttribute jpa
                ? jpa.PropertyName
                : expression.Member.Name;

            builder.Append(name + ": ");
        }

        public static string Listener(StringBuilder builder, Expression expression)
        {
            throw new NotImplementedException($"Expression node {expression.NodeType} not supported");
        }
    }

    public class NodeSet<T> where T : class
    {
        // this will perform the following actions:
        // 1. find node label; this will be `typeof(T)`, find `Label` attribute, use `Name` property: OR use type name if no `Label` attribute
        // 2. evaluate expression into json match; todo: create/apply attribute to override property name
        // 3. sanitize match expression, if necessary
        // 4. build and run query expression
        // 5. deserialize and return response
        public async ValueTask<T?> Find(Expression<Func<T, bool>> expression)
        {
            var client = new Neo4jClient.GraphClient(new Uri("http://localhost:7474"));
            await client.ConnectAsync();

            var label = typeof(T).GetCustomAttributes(typeof(LabelAttribute), false).FirstOrDefault() is LabelAttribute labelAttr
                ? labelAttr.Name
                : typeof(T).Name;

            var filter = ExpressionParser.Parse(expression);

            var query = client.Cypher
                .Match($"(data: {label} {filter})")
                .Return((data) => new
                {
                    data = Return.As<T>("data")
                })
                .Limit(1);

            return (await query.ResultsAsync).FirstOrDefault()?.data;
        }

        // this will perform the following actions:
        // 1. if an expression is provided
        //    1. find node label; this will be `typeof(T)`, find `Label` attribute, use `Name` property: OR use type name if no `Label` attribute
        //    2. evaluate expression into json match; todo: create/apply attribute to override property name
        //    3. sanitize match expression, if necessary
        // 2. build and run query expression
        // 3. deserialize and return response
        public async ValueTask<IReadOnlyList<T>> FindAll(Expression<Func<T, bool>>? expression = null)
        {
            var client = new Neo4jClient.GraphClient(new Uri("http://localhost:7474"));
            await client.ConnectAsync();

            var label = typeof(T).GetCustomAttributes(typeof(LabelAttribute), false).FirstOrDefault() is LabelAttribute labelAttr
                ? labelAttr.Name
                : typeof(T).Name;

            var query = client.Cypher
                .Match($"(data: {label})")
                .Return((data) => new
                {
                    data = Return.As<T>("data")
                });

            return (await query.ResultsAsync).Select(s => s.data).ToList()!;
        }

        // this will perform the following actions:
        // 1. if an expression is provided
        //    1. find node label; this will be `typeof(T)`, find `Label` attribute, use `Name` property: OR use type name if no `Label` attribute
        //    2. evaluate expression into json match; todo: create/apply attribute to override property name
        //    3. sanitize match expression, if necessary
        // 2. build and run query expression
        // 3. deserialize and return response
        public async ValueTask<long> Count(Expression<Func<T, bool>>? expression = null)
        {
            var client = new Neo4jClient.GraphClient(new Uri("http://localhost:7474"));
            await client.ConnectAsync();

            var label = typeof(T).GetCustomAttributes(typeof(LabelAttribute), false).FirstOrDefault() is LabelAttribute labelAttr
                ? labelAttr.Name
                : typeof(T).Name;

            var query = client.Cypher
                .Match($"(data: {label})")
                .Return((data) => new
                {
                    data = data.Count()
                });

            return (await query.ResultsAsync).First().data;
        }
    }
}