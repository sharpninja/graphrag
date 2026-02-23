// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Config.Models;

/// <summary>
/// Configuration for pipeline snapshots.
/// </summary>
public sealed record SnapshotsConfig
{
    /// <summary>Gets a value indicating whether to snapshot embeddings.</summary>
    public bool Embeddings { get; init; }

    /// <summary>Gets a value indicating whether to snapshot graphs in GraphML format.</summary>
    public bool GraphMl { get; init; }

    /// <summary>Gets a value indicating whether to snapshot the raw graph.</summary>
    public bool RawGraph { get; init; }
}
