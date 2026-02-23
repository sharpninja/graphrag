// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Config.Models;

/// <summary>
/// Configuration for graph clustering.
/// </summary>
public sealed record ClusterGraphConfig
{
    /// <summary>Gets the maximum cluster size.</summary>
    public int MaxClusterSize { get; init; } = 10;

    /// <summary>Gets a value indicating whether to use the largest connected component.</summary>
    public bool UseLcc { get; init; } = true;

    /// <summary>Gets the seed for deterministic clustering.</summary>
    public uint Seed { get; init; } = 0xDEADBEEF;
}
