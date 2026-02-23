// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.DataModel;

/// <summary>
/// A community of entities within the knowledge graph.
/// </summary>
public sealed record Community : Named
{
    /// <summary>
    /// Gets the level of the community.
    /// </summary>
    public required string Level { get; init; }

    /// <summary>
    /// Gets the parent community identifier.
    /// </summary>
    public required string Parent { get; init; }

    /// <summary>
    /// Gets the child community identifiers.
    /// </summary>
    public required IReadOnlyList<string> Children { get; init; }

    /// <summary>
    /// Gets the entity identifiers belonging to this community.
    /// </summary>
    public IReadOnlyList<string>? EntityIds { get; init; }

    /// <summary>
    /// Gets the relationship identifiers belonging to this community.
    /// </summary>
    public IReadOnlyList<string>? RelationshipIds { get; init; }

    /// <summary>
    /// Gets the text unit identifiers associated with this community.
    /// </summary>
    public IReadOnlyList<string>? TextUnitIds { get; init; }

    /// <summary>
    /// Gets the covariate identifiers associated with this community.
    /// </summary>
    public Dictionary<string, IReadOnlyList<string>>? CovariateIds { get; init; }

    /// <summary>
    /// Gets the additional attributes of the community.
    /// </summary>
    public Dictionary<string, object?>? Attributes { get; init; }

    /// <summary>
    /// Gets the size of the community.
    /// </summary>
    public int? Size { get; init; }

    /// <summary>
    /// Gets the period of the community.
    /// </summary>
    public string? Period { get; init; }

    /// <summary>
    /// Creates a <see cref="Community"/> from a dictionary of values.
    /// </summary>
    /// <param name="data">The dictionary containing community data.</param>
    /// <returns>A new <see cref="Community"/> instance.</returns>
    public static Community FromDictionary(Dictionary<string, object?> data)
    {
        return new Community
        {
            Id = data.TryGetValue("id", out var id) ? id?.ToString() ?? string.Empty : string.Empty,
            ShortId = data.TryGetValue("short_id", out var shortId) ? shortId?.ToString() : null,
            Title = data.TryGetValue("title", out var title) ? title?.ToString() ?? string.Empty : string.Empty,
            Level = data.TryGetValue("level", out var level) ? level?.ToString() ?? string.Empty : string.Empty,
            Parent = data.TryGetValue("parent", out var parent) ? parent?.ToString() ?? string.Empty : string.Empty,
            Children = data.TryGetValue("children", out var children) ? children as IReadOnlyList<string> ?? [] : [],
            EntityIds = data.TryGetValue("entity_ids", out var eIds) ? eIds as IReadOnlyList<string> : null,
            RelationshipIds = data.TryGetValue("relationship_ids", out var rIds) ? rIds as IReadOnlyList<string> : null,
            TextUnitIds = data.TryGetValue("text_unit_ids", out var tuIds) ? tuIds as IReadOnlyList<string> : null,
            Size = data.TryGetValue("size", out var size) && size is int s ? s : null,
            Period = data.TryGetValue("period", out var period) ? period?.ToString() : null,
        };
    }
}
