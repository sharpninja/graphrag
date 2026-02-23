// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Vectors;

/// <summary>
/// Represents a search result from a vector store similarity search.
/// </summary>
/// <param name="Document">The matching document.</param>
/// <param name="Score">The similarity score.</param>
public sealed record VectorStoreSearchResult(
    VectorStoreDocument Document,
    double Score);
