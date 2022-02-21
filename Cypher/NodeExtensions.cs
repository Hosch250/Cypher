namespace Cypher
{
    using Neo4jClient.Cypher;
    using System.Runtime.CompilerServices;

    public static class NodeExtensions
    {
        public static IReadOnlyList<TResponse> GetNodesByRelationship<TResponse>(this Node node, [CallerMemberName] string callerMemberName = "")
            where TResponse : Node
        {
            var client = new Neo4jClient.GraphClient(new Uri("http://localhost:7474"));
            client.ConnectAsync().Wait();

            var relationship = node.GetType().GetProperty(callerMemberName)!.GetCustomAttributes(typeof(RelationshipAttribute), false).FirstOrDefault() as RelationshipAttribute;
            if (relationship is null) { throw new Exception(); }

            var fromType = relationship.Direction == "FROM" ? node.GetType() : typeof(TResponse);
            var fromLabel = fromType.GetCustomAttributes(typeof(LabelAttribute), false).FirstOrDefault() is LabelAttribute labelAttr
                ? labelAttr.Name
                : fromType.Name;

            var toType = relationship.Direction == "FROM" ? typeof(TResponse) : node.GetType();
            var toLabel = toType.GetCustomAttributes(typeof(LabelAttribute), false).FirstOrDefault() is LabelAttribute labelAttr1
                ? labelAttr1.Name
                : toType.Name;

            fromLabel = relationship.Direction == "FROM" ? $"known:{fromLabel}" : $"data:{fromLabel}";
            toLabel = relationship.Direction == "TO" ? $"known:{toLabel}" : $"data:{toLabel}";

            var query = client.Cypher
                .Match($"({fromLabel})-[:{relationship.Relationship}]->({toLabel})")
                .Where($"id(known) = {node.Identity}")
                .Return((data) => new
                {
                    data = data.As<TResponse>(),
                    id = data.Id()
                });

            return query.ResultsAsync.Result.Select(s => {
                s.data.Identity = s.id;
                return s.data;
            }).ToList()!;
        }

        public static IReadOnlyList<(TRelationshipResponse relationship, TNodeResponse node)> GetNodesWithRelationship<TRelationshipResponse, TNodeResponse>(this Node node, [CallerMemberName] string callerMemberName = "")
            where TNodeResponse : Node
            where TRelationshipResponse : Node
        {
            var client = new Neo4jClient.GraphClient(new Uri("http://localhost:7474"));
            client.ConnectAsync().Wait();

            var relationship = node.GetType().GetProperty(callerMemberName)!.GetCustomAttributes(typeof(RelationshipAttribute), false).FirstOrDefault() as RelationshipAttribute;
            if (relationship is null) { throw new Exception(); }

            var fromType = relationship.Direction == "FROM" ? node.GetType() : typeof(TNodeResponse);
            var fromLabel = fromType.GetCustomAttributes(typeof(LabelAttribute), false).FirstOrDefault() is LabelAttribute labelAttr
                ? labelAttr.Name
                : fromType.Name;

            var toType = relationship.Direction == "FROM" ? typeof(TNodeResponse) : node.GetType();
            var toLabel = toType.GetCustomAttributes(typeof(LabelAttribute), false).FirstOrDefault() is LabelAttribute labelAttr1
                ? labelAttr1.Name
                : toType.Name;

            fromLabel = relationship.Direction == "FROM" ? $"known:{fromLabel}" : $"data:{fromLabel}";
            toLabel = relationship.Direction == "TO" ? $"known:{toLabel}" : $"data:{toLabel}";

            var query = client.Cypher
                .Match($"({fromLabel})-[r:{relationship.Relationship}]->({toLabel})")
                .Where($"id(known) = {node.Identity}")
                .Return((r, data) => new
                {
                    node = data.As<TNodeResponse>(),
                    relationship = r.As<TRelationshipResponse>(),
                    nodeId = data.Id(),
                    relationshipId = r.Id()
                });

            return query.ResultsAsync.Result.Select(s => {
                s.node.Identity = s.nodeId;
                s.relationship.Identity = s.relationshipId;
                return (s.relationship, s.node);
            }).ToList();
        }
    }
}