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

    /// <summary>
    /// Creates a <see cref="CommunityReport"/> from a dictionary of values.
    /// </summary>
    /// <param name="data">The dictionary containing community report data.</param>
    /// <returns>A new <see cref="CommunityReport"/> instance.</returns>
    public static CommunityReport FromDictionary(Dictionary<string, object?> data)
    {
        return new CommunityReport
        {
            Id = data.TryGetValue("id", out var id) ? id?.ToString() ?? string.Empty : string.Empty,
            ShortId = data.TryGetValue("short_id", out var shortId) ? shortId?.ToString() : null,
            Title = data.TryGetValue("title", out var title) ? title?.ToString() ?? string.Empty : string.Empty,
            CommunityId = data.TryGetValue("community_id", out var cid) ? cid?.ToString() ?? string.Empty : string.Empty,
            Summary = data.TryGetValue("summary", out var summary) ? summary?.ToString() ?? string.Empty : string.Empty,
            FullContent = data.TryGetValue("full_content", out var fc) ? fc?.ToString() ?? string.Empty : string.Empty,
            Rank = data.TryGetValue("rank", out var rank) && rank is double r ? r : 1.0,
            Size = data.TryGetValue("size", out var size) && size is int s ? s : null,
            Period = data.TryGetValue("period", out var period) ? period?.ToString() : null,
        };
    }
}
