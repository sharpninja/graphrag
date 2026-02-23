// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Graph.QuikGraph;

/// <summary>
/// Defines an interface for graph algorithm operations.
/// </summary>
/// <remarks>
/// This interface is defined locally until it is refactored into a shared
/// abstractions project (e.g., GraphRag.Common or GraphRag.Graph.Abstractions).
/// </remarks>
public interface IGraphAlgorithms
{
    /// <summary>
    /// Gets the connected components of a graph defined by the given edges.
    /// </summary>
    /// <param name="edges">The list of edges, each represented as a source-target tuple.</param>
    /// <returns>A list of connected components, each being a list of node identifiers.</returns>
    IReadOnlyList<IReadOnlyList<string>> GetConnectedComponents(IReadOnlyList<(string Source, string Target)> edges);

    /// <summary>
    /// Computes the degree (number of connections) for each node in the graph.
    /// </summary>
    /// <param name="edges">The list of edges, each represented as a source-target tuple.</param>
    /// <returns>A dictionary mapping each node identifier to its degree.</returns>
    IReadOnlyDictionary<string, int> ComputeNodeDegrees(IReadOnlyList<(string Source, string Target)> edges);
}
