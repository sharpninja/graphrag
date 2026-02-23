// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Config.Defaults;

namespace GraphRag.Config.Models;

/// <summary>
/// Configuration for basic vector-similarity search.
/// </summary>
public sealed record BasicSearchConfig
{
    /// <summary>Gets the prompt template to use for basic search.</summary>
    public string? Prompt { get; init; }

    /// <summary>Gets the completion model identifier.</summary>
    public string CompletionModelId { get; init; } = DefaultValues.DefaultCompletionModelId;

    /// <summary>Gets the embedding model identifier.</summary>
    public string EmbeddingModelId { get; init; } = DefaultValues.DefaultEmbeddingModelId;

    /// <summary>Gets the number of nearest neighbors to retrieve.</summary>
    public int K { get; init; } = 10;

    /// <summary>Gets the maximum number of context tokens to use.</summary>
    public int MaxContextTokens { get; init; } = 12_000;
}
