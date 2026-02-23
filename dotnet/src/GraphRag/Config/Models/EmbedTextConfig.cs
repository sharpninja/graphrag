// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Config.Defaults;

namespace GraphRag.Config.Models;

/// <summary>
/// Configuration for text embedding generation.
/// </summary>
public sealed record EmbedTextConfig
{
    /// <summary>Gets the embedding model identifier.</summary>
    public string EmbeddingModelId { get; init; } = DefaultValues.DefaultEmbeddingModelId;

    /// <summary>Gets the model instance name for this workflow step.</summary>
    public string ModelInstanceName { get; init; } = "text_embedding";

    /// <summary>Gets the batch size for embedding requests.</summary>
    public int BatchSize { get; init; } = 16;

    /// <summary>Gets the maximum number of tokens per batch.</summary>
    public int BatchMaxTokens { get; init; } = 8191;

    /// <summary>Gets the list of embedding names to generate.</summary>
    public IReadOnlyList<string> Names { get; init; } = [Embeddings.EntityDescription, Embeddings.CommunityFullContent, Embeddings.TextUnitText];
}
