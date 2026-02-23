// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.DataModel;

/// <summary>
/// A relationship between two entities.
/// </summary>
public sealed record Relationship : Identified
{
    /// <summary>
    /// Gets the source entity identifier.
    /// </summary>
    public required string Source { get; init; }

    /// <summary>
    /// Gets the target entity identifier.
    /// </summary>
    public required string Target { get; init; }

    /// <summary>
    /// Gets the weight of the relationship.
    /// </summary>
    public double? Weight { get; init; } = 1.0;

    /// <summary>
    /// Gets the description of the relationship.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the embedding for the relationship description.
    /// </summary>
    public IReadOnlyList<float>? DescriptionEmbedding { get; init; }

    /// <summary>
    /// Gets the text unit identifiers associated with the relationship.
    /// </summary>
    public IReadOnlyList<string>? TextUnitIds { get; init; }

    /// <summary>
    /// Gets the rank of the relationship.
    /// </summary>
    public int? Rank { get; init; } = 1;

    /// <summary>
    /// Gets the additional attributes of the relationship.
    /// </summary>
    public Dictionary<string, object?>? Attributes { get; init; }

    /// <summary>
    /// Creates a <see cref="Relationship"/> from a dictionary of values.
    /// </summary>
    /// <param name="data">The dictionary containing relationship data.</param>
    /// <returns>A new <see cref="Relationship"/> instance.</returns>
    public static Relationship FromDictionary(Dictionary<string, object?> data)
    {
        return new Relationship
        {
            Id = data.TryGetValue("id", out var id) ? id?.ToString() ?? string.Empty : string.Empty,
            ShortId = data.TryGetValue("short_id", out var shortId) ? shortId?.ToString() : null,
            Source = data.TryGetValue("source", out var source) ? source?.ToString() ?? string.Empty : string.Empty,
            Target = data.TryGetValue("target", out var target) ? target?.ToString() ?? string.Empty : string.Empty,
            Weight = data.TryGetValue("weight", out var weight) && weight is double w ? w : 1.0,
            Description = data.TryGetValue("description", out var desc) ? desc?.ToString() : null,
            DescriptionEmbedding = data.TryGetValue("description_embedding", out var descEmb) ? descEmb as IReadOnlyList<float> : null,
            TextUnitIds = data.TryGetValue("text_unit_ids", out var tuIds) ? tuIds as IReadOnlyList<string> : null,
            Rank = data.TryGetValue("rank", out var rank) && rank is int r ? r : 1,
            Attributes = data.TryGetValue("attributes", out var attrs) ? attrs as Dictionary<string, object?> : null,
        };
    }
}
