// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.DataModel;

namespace GraphRag.Index.Operations;

/// <summary>
/// Utility methods for graph analysis.
/// </summary>
public static class GraphUtils
{
    /// <summary>
    /// Computes the degree of a node in the graph based on its relationships.
    /// </summary>
    /// <param name="nodeId">The identifier of the node.</param>
    /// <param name="relationships">The relationships to search.</param>
    /// <returns>The number of relationships involving the specified node.</returns>
    public static int ComputeNodeDegree(string nodeId, IReadOnlyList<Relationship> relationships)
    {
        int degree = 0;
        for (int i = 0; i < relationships.Count; i++)
        {
            var rel = relationships[i];
            if (string.Equals(rel.Source, nodeId, StringComparison.Ordinal) ||
                string.Equals(rel.Target, nodeId, StringComparison.Ordinal))
            {
                degree++;
            }
        }

        return degree;
    }

    /// <summary>
    /// Finds connected components in the graph using union-find.
    /// </summary>
    /// <param name="entities">The entities (nodes) in the graph.</param>
    /// <param name="relationships">The relationships (edges) in the graph.</param>
    /// <returns>A list of connected components, each being a list of entity identifiers.</returns>
    public static IReadOnlyList<IReadOnlyList<string>> FindConnectedComponents(
        IReadOnlyList<Entity> entities,
        IReadOnlyList<Relationship> relationships)
    {
        var parent = new Dictionary<string, string>();

        string Find(string x)
        {
            if (!parent.TryGetValue(x, out _))
            {
                parent[x] = x;
            }

            while (parent[x] != x)
            {
                parent[x] = parent[parent[x]];
                x = parent[x];
            }

            return x;
        }

        void Union(string a, string b)
        {
            var ra = Find(a);
            var rb = Find(b);
            if (ra != rb)
            {
                parent[ra] = rb;
            }
        }

        foreach (var entity in entities)
        {
            Find(entity.Id);
        }

        foreach (var rel in relationships)
        {
            if (parent.ContainsKey(rel.Source) && parent.ContainsKey(rel.Target))
            {
                Union(rel.Source, rel.Target);
            }
        }

        var components = new Dictionary<string, List<string>>();
        foreach (var entityId in parent.Keys)
        {
            var root = Find(entityId);
            if (!components.TryGetValue(root, out var list))
            {
                list = [];
                components[root] = list;
            }

            list.Add(entityId);
        }

        return components.Values
            .Select(c => (IReadOnlyList<string>)c.AsReadOnly())
            .ToList()
            .AsReadOnly();
    }
}
