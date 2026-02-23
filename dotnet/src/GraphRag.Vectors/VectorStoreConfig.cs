// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Vectors;

/// <summary>
/// Configuration for a vector store connection.
/// </summary>
public sealed record VectorStoreConfig
{
    /// <summary>
    /// Gets the type of vector store.
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// Gets the database URI.
    /// </summary>
    public string? DbUri { get; init; }

    /// <summary>
    /// Gets the service URL.
    /// </summary>
    public string? Url { get; init; }

    /// <summary>
    /// Gets the API key for authentication.
    /// </summary>
    public string? ApiKey { get; init; }

    /// <summary>
    /// Gets the audience for token-based authentication.
    /// </summary>
    public string? Audience { get; init; }

    /// <summary>
    /// Gets the connection string.
    /// </summary>
    public string? ConnectionString { get; init; }

    /// <summary>
    /// Gets the database name.
    /// </summary>
    public string? DatabaseName { get; init; }

    /// <summary>
    /// Gets the size of the vectors stored.
    /// </summary>
    public int VectorSize { get; init; } = 3072;

    /// <summary>
    /// Gets the index schema configuration.
    /// </summary>
    public IndexSchema? IndexSchema { get; init; }
}
