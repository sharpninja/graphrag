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
}
