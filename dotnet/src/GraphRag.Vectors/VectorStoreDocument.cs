// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Vectors;

/// <summary>
/// Represents a document stored in a vector store.
/// </summary>
public sealed record VectorStoreDocument
{
    /// <summary>
    /// Gets the unique identifier of the document.
    /// </summary>
    public required string Id { get; init; }

    /// <summary>
    /// Gets the embedding vector associated with the document.
    /// </summary>
    public IReadOnlyList<float>? Vector { get; init; }

    /// <summary>
    /// Gets the data fields associated with the document.
    /// </summary>
    public Dictionary<string, object?> Data { get; init; } = [];

    /// <summary>
    /// Gets the creation date of the document in ISO 8601 format.
    /// </summary>
    public string? CreateDate { get; init; }

    /// <summary>
    /// Gets the last update date of the document in ISO 8601 format.
    /// </summary>
    public string? UpdateDate { get; init; }
}
