// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Config.Defaults;

namespace GraphRag.Config.Models;

/// <summary>
/// Configuration for DRIFT (Dynamic Reasoning and Inference with Flexible Traversal) search.
/// </summary>
public sealed record DriftSearchConfig
{
    /// <summary>Gets the prompt template for DRIFT search.</summary>
    public string? Prompt { get; init; }

    /// <summary>Gets the reduce prompt template for DRIFT search.</summary>
    public string? ReducePrompt { get; init; }

    /// <summary>Gets the completion model identifier.</summary>
    public string CompletionModelId { get; init; } = DefaultValues.DefaultCompletionModelId;

    /// <summary>Gets the embedding model identifier.</summary>
    public string EmbeddingModelId { get; init; } = DefaultValues.DefaultEmbeddingModelId;

    /// <summary>Gets the maximum number of data tokens.</summary>
    public int DataMaxTokens { get; init; } = 12_000;

    /// <summary>Gets the maximum number of tokens for the reduce step.</summary>
    public int? ReduceMaxTokens { get; init; }

    /// <summary>Gets the temperature for the reduce step.</summary>
    public double ReduceTemperature { get; init; }

    /// <summary>Gets the maximum completion tokens for the reduce step.</summary>
    public int? ReduceMaxCompletionTokens { get; init; }

    /// <summary>Gets the concurrency level for DRIFT search.</summary>
    public int Concurrency { get; init; } = 32;

    /// <summary>Gets the number of follow-up queries to generate.</summary>
    public int DriftKFollowups { get; init; } = 20;

    /// <summary>Gets the number of primer folds.</summary>
    public int PrimerFolds { get; init; } = 5;

    /// <summary>Gets the maximum number of tokens for the primer LLM call.</summary>
    public int PrimerLlmMaxTokens { get; init; } = 12_000;

    /// <summary>Gets the search depth.</summary>
    public int NDepth { get; init; } = 3;

    /// <summary>Gets the proportion of text units for local search.</summary>
    public double LocalSearchTextUnitProp { get; init; } = 0.9;

    /// <summary>Gets the proportion of community data for local search.</summary>
    public double LocalSearchCommunityProp { get; init; } = 0.1;

    /// <summary>Gets the number of top-k mapped entities for local search.</summary>
    public int LocalSearchTopKMappedEntities { get; init; } = 10;

    /// <summary>Gets the number of top-k relationships for local search.</summary>
    public int LocalSearchTopKRelationships { get; init; } = 10;

    /// <summary>Gets the maximum number of data tokens for local search.</summary>
    public int LocalSearchMaxDataTokens { get; init; } = 12_000;

    /// <summary>Gets the temperature for local search.</summary>
    public double LocalSearchTemperature { get; init; }

    /// <summary>Gets the top-p value for local search.</summary>
    public double LocalSearchTopP { get; init; } = 1.0;

    /// <summary>Gets the number of responses for local search.</summary>
    public int LocalSearchN { get; init; } = 1;

    /// <summary>Gets the maximum generation tokens for local search LLM.</summary>
    public int? LocalSearchLlmMaxGenTokens { get; init; }

    /// <summary>Gets the maximum completion tokens for local search LLM.</summary>
    public int? LocalSearchLlmMaxGenCompletionTokens { get; init; }
}
