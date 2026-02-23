// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.DataModel;

/// <summary>
/// A protocol for an item with a name/title.
/// </summary>
public record Named : Identified
{
    /// <summary>
    /// Gets the title of the item.
    /// </summary>
    public required string Title { get; init; }
}
