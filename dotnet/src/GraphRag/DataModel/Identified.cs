// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.DataModel;

/// <summary>
/// A protocol for an item with an ID.
/// </summary>
public record Identified
{
    /// <summary>
    /// Gets the unique identifier of the item.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Gets the short identifier of the item.
    /// </summary>
    public string? ShortId { get; init; }
}
