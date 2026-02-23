// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Common.Discovery;

using QuikGraph;
using QuikGraph.Algorithms.ConnectedComponents;

namespace GraphRag.Graph.QuikGraph;

/// <summary>
/// Provides graph algorithms backed by the QuikGraph library.
/// </summary>
[StrategyImplementation("quikgraph", typeof(IGraphAlgorithms))]
public sealed class QuikGraphAlgorithms : IGraphAlgorithms
{
    /// <inheritdoc />
    public IReadOnlyList<IReadOnlyList<string>> GetConnectedComponents(
        IReadOnlyList<(string Source, string Target)> edges)
    {
        var graph = BuildUndirectedGraph(edges);
        var algorithm = new ConnectedComponentsAlgorithm<string, UndirectedEdge<string>>(graph);
        algorithm.Compute();

        var componentGroups = new Dictionary<int, List<string>>();
        foreach (var kvp in algorithm.Components)
        {
            if (!componentGroups.TryGetValue(kvp.Value, out var list))
            {
                list = [];
                componentGroups[kvp.Value] = list;
            }

            list.Add(kvp.Key);
        }

        return componentGroups.Values
            .Select(g => (IReadOnlyList<string>)g.AsReadOnly())
            .ToList()
            .AsReadOnly();
    }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, int> ComputeNodeDegrees(
        IReadOnlyList<(string Source, string Target)> edges)
    {
        var degrees = new Dictionary<string, int>();
        foreach (var (source, target) in edges)
        {
            degrees[source] = degrees.GetValueOrDefault(source) + 1;
            degrees[target] = degrees.GetValueOrDefault(target) + 1;
        }

        return degrees;
    }

    private static UndirectedGraph<string, UndirectedEdge<string>> BuildUndirectedGraph(
        IReadOnlyList<(string Source, string Target)> edges)
    {
        var graph = new UndirectedGraph<string, UndirectedEdge<string>>();
        foreach (var (source, target) in edges)
        {
            graph.AddVerticesAndEdge(new UndirectedEdge<string>(source, target));
        }

        return graph;
    }
}
