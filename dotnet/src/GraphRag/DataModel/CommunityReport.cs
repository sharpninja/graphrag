// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.DataModel;

/// <summary>
/// A report generated for a community.
/// </summary>
public sealed record CommunityReport : Named
{
    /// <summary>
    /// Gets the community identifier this report belongs to.
    /// </summary>
    public required string CommunityId { get; init; }

    /// <summary>
    /// Gets the summary of the community report.
    /// </summary>
    public string Summary { get; init; } = string.Empty;

    /// <summary>
    /// Gets the full content of the community report.
    /// </summary>
    public string FullContent { get; init; } = string.Empty;

    /// <summary>
    /// Gets the rank of the community report.
    /// </summary>
    public double? Rank { get; init; } = 1.0;

    /// <summary>
    /// Gets the embedding for the full content of the report.
    /// </summary>
    public IReadOnlyList<float>? FullContentEmbedding { get; init; }

    /// <summary>
    /// Gets the additional attributes of the community report.
    /// </summary>
    public Dictionary<string, object?>? Attributes { get; init; }

    /// <summary>
    /// Gets the size of the community report.
    /// </summary>
    public int? Size { get; init; }

    /// <summary>
    /// Gets the period of the community report.
    /// </summary>
    public string? Period { get; init; }
}
