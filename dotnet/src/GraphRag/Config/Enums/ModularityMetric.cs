// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Config.Enums;

/// <summary>
/// Available modularity metric types for graph clustering.
/// </summary>
public static class ModularityMetric
{
    /// <summary>Gets the identifier for full graph modularity.</summary>
    public const string Graph = "graph";

    /// <summary>Gets the identifier for largest connected component modularity.</summary>
    public const string Lcc = "lcc";

    /// <summary>Gets the identifier for weighted components modularity.</summary>
    public const string WeightedComponents = "weighted_components";
}
