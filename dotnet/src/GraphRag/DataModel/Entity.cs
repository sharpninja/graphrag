// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.DataModel;

/// <summary>
/// An entity extracted from a document.
/// </summary>
public sealed record Entity : Named
{
    /// <summary>
    /// Gets the type of the entity.
    /// </summary>
    public string? Type { get; init; }

    /// <summary>
    /// Gets the description of the entity.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the embedding for the entity description.
    /// </summary>
    public IReadOnlyList<float>? DescriptionEmbedding { get; init; }

    /// <summary>
    /// Gets the embedding for the entity name.
    /// </summary>
    public IReadOnlyList<float>? NameEmbedding { get; init; }

    /// <summary>
    /// Gets the community identifiers associated with the entity.
    /// </summary>
    public IReadOnlyList<string>? CommunityIds { get; init; }

    /// <summary>
    /// Gets the text unit identifiers associated with the entity.
    /// </summary>
    public IReadOnlyList<string>? TextUnitIds { get; init; }

    /// <summary>
    /// Gets the rank of the entity.
    /// </summary>
    public int? Rank { get; init; } = 1;

    /// <summary>
    /// Gets the additional attributes of the entity.
    /// </summary>
    public Dictionary<string, object?>? Attributes { get; init; }

    /// <summary>
    /// Creates an <see cref="Entity"/> from a dictionary of values.
    /// </summary>
    /// <param name="data">The dictionary containing entity data.</param>
    /// <returns>A new <see cref="Entity"/> instance.</returns>
    public static Entity FromDictionary(Dictionary<string, object?> data)
    {
        return new Entity
        {
            Id = data.TryGetValue("id", out var id) ? id?.ToString() ?? string.Empty : string.Empty,
            ShortId = data.TryGetValue("short_id", out var shortId) ? shortId?.ToString() : null,
            Title = data.TryGetValue("title", out var title) ? title?.ToString() ?? string.Empty : string.Empty,
            Type = data.TryGetValue("type", out var type) ? type?.ToString() : null,
            Description = data.TryGetValue("description", out var desc) ? desc?.ToString() : null,
            DescriptionEmbedding = data.TryGetValue("description_embedding", out var descEmb) ? descEmb as IReadOnlyList<float> : null,
            NameEmbedding = data.TryGetValue("name_embedding", out var nameEmb) ? nameEmb as IReadOnlyList<float> : null,
            CommunityIds = data.TryGetValue("community_ids", out var commIds) ? commIds as IReadOnlyList<string> : null,
            TextUnitIds = data.TryGetValue("text_unit_ids", out var tuIds) ? tuIds as IReadOnlyList<string> : null,
            Rank = data.TryGetValue("rank", out var rank) && rank is int r ? r : 1,
            Attributes = data.TryGetValue("attributes", out var attrs) ? attrs as Dictionary<string, object?> : null,
        };
    }
}
