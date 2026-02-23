// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.DataModel;

/// <summary>
/// A unit of text extracted from a document.
/// </summary>
public sealed record TextUnit : Identified
{
    /// <summary>
    /// Gets the text content of the unit.
    /// </summary>
    public required string Text { get; init; }

    /// <summary>
    /// Gets the entity identifiers associated with this text unit.
    /// </summary>
    public IReadOnlyList<string>? EntityIds { get; init; }

    /// <summary>
    /// Gets the relationship identifiers associated with this text unit.
    /// </summary>
    public IReadOnlyList<string>? RelationshipIds { get; init; }

    /// <summary>
    /// Gets the covariate identifiers associated with this text unit.
    /// </summary>
    public Dictionary<string, IReadOnlyList<string>>? CovariateIds { get; init; }

    /// <summary>
    /// Gets the number of tokens in the text unit.
    /// </summary>
    public int? NTokens { get; init; }

    /// <summary>
    /// Gets the document identifier this text unit belongs to.
    /// </summary>
    public string? DocumentId { get; init; }

    /// <summary>
    /// Gets the additional attributes of the text unit.
    /// </summary>
    public Dictionary<string, object?>? Attributes { get; init; }
}
