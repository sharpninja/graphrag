// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Config.Models;

/// <summary>
/// Configuration for graph pruning.
/// </summary>
public sealed record PruneGraphConfig
{
    /// <summary>Gets the minimum node frequency threshold.</summary>
    public int MinNodeFreq { get; init; } = 2;

    /// <summary>Gets the maximum node frequency standard deviation threshold.</summary>
    public double? MaxNodeFreqStd { get; init; }

    /// <summary>Gets the minimum node degree threshold.</summary>
    public int MinNodeDegree { get; init; } = 1;

    /// <summary>Gets the maximum node degree standard deviation threshold.</summary>
    public double? MaxNodeDegreeStd { get; init; }

    /// <summary>Gets the minimum edge weight percentile threshold.</summary>
    public double MinEdgeWeightPct { get; init; } = 40.0;

    /// <summary>Gets a value indicating whether to remove ego nodes.</summary>
    public bool RemoveEgoNodes { get; init; } = true;

    /// <summary>Gets a value indicating whether to keep only the largest connected component.</summary>
    public bool LccOnly { get; init; }
}
