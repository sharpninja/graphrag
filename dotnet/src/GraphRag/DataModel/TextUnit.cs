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

    /// <summary>
    /// Creates a <see cref="TextUnit"/> from a dictionary of values.
    /// </summary>
    /// <param name="data">The dictionary containing text unit data.</param>
    /// <returns>A new <see cref="TextUnit"/> instance.</returns>
    public static TextUnit FromDictionary(Dictionary<string, object?> data)
    {
        return new TextUnit
        {
            Id = data.TryGetValue("id", out var id) ? id?.ToString() ?? string.Empty : string.Empty,
            ShortId = data.TryGetValue("short_id", out var shortId) ? shortId?.ToString() : null,
            Text = data.TryGetValue("text", out var text) ? text?.ToString() ?? string.Empty : string.Empty,
            EntityIds = data.TryGetValue("entity_ids", out var eIds) ? eIds as IReadOnlyList<string> : null,
            RelationshipIds = data.TryGetValue("relationship_ids", out var rIds) ? rIds as IReadOnlyList<string> : null,
            NTokens = data.TryGetValue("n_tokens", out var nt) && nt is int n ? n : null,
            DocumentId = data.TryGetValue("document_id", out var did) ? did?.ToString() : null,
            Attributes = data.TryGetValue("attributes", out var attrs) ? attrs as Dictionary<string, object?> : null,
        };
    }
}
