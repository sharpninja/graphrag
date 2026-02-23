// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.DataModel;

/// <summary>
/// A document in the knowledge graph.
/// </summary>
public sealed record Document : Named
{
    /// <summary>
    /// Gets the type of the document.
    /// </summary>
    public string Type { get; init; } = "text";

    /// <summary>
    /// Gets the text unit identifiers associated with this document.
    /// </summary>
    public IReadOnlyList<string> TextUnitIds { get; init; } = [];

    /// <summary>
    /// Gets the text content of the document.
    /// </summary>
    public string Text { get; init; } = string.Empty;

    /// <summary>
    /// Gets the additional attributes of the document.
    /// </summary>
    public Dictionary<string, object?>? Attributes { get; init; }

    /// <summary>
    /// Creates a <see cref="Document"/> from a dictionary of values.
    /// </summary>
    /// <param name="data">The dictionary containing document data.</param>
    /// <returns>A new <see cref="Document"/> instance.</returns>
    public static Document FromDictionary(Dictionary<string, object?> data)
    {
        return new Document
        {
            Id = data.TryGetValue("id", out var id) ? id?.ToString() ?? string.Empty : string.Empty,
            ShortId = data.TryGetValue("short_id", out var shortId) ? shortId?.ToString() : null,
            Title = data.TryGetValue("title", out var title) ? title?.ToString() ?? string.Empty : string.Empty,
            Type = data.TryGetValue("type", out var type) ? type?.ToString() ?? "text" : "text",
            Text = data.TryGetValue("text", out var text) ? text?.ToString() ?? string.Empty : string.Empty,
            TextUnitIds = data.TryGetValue("text_unit_ids", out var tuIds) ? tuIds as IReadOnlyList<string> ?? [] : [],
            Attributes = data.TryGetValue("attributes", out var attrs) ? attrs as Dictionary<string, object?> : null,
        };
    }
}
