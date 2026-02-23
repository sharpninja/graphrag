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
}
