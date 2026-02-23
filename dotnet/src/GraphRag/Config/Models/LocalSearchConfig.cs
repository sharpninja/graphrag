// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Config.Defaults;

namespace GraphRag.Config.Models;

/// <summary>
/// Configuration for local context-based search.
/// </summary>
public sealed record LocalSearchConfig
{
    /// <summary>Gets the prompt template for local search.</summary>
    public string? Prompt { get; init; }

    /// <summary>Gets the completion model identifier.</summary>
    public string CompletionModelId { get; init; } = DefaultValues.DefaultCompletionModelId;

    /// <summary>Gets the embedding model identifier.</summary>
    public string EmbeddingModelId { get; init; } = DefaultValues.DefaultEmbeddingModelId;

    /// <summary>Gets the proportion of context allocated to text units.</summary>
    public double TextUnitProp { get; init; } = 0.5;

    /// <summary>Gets the proportion of context allocated to community data.</summary>
    public double CommunityProp { get; init; } = 0.15;

    /// <summary>Gets the maximum number of conversation history turns to include.</summary>
    public int ConversationHistoryMaxTurns { get; init; } = 5;

    /// <summary>Gets the number of top-k entities to retrieve.</summary>
    public int TopKEntities { get; init; } = 10;

    /// <summary>Gets the number of top-k relationships to retrieve.</summary>
    public int TopKRelationships { get; init; } = 10;

    /// <summary>Gets the maximum number of context tokens.</summary>
    public int MaxContextTokens { get; init; } = 12_000;
}
