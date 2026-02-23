// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Config.Defaults;

namespace GraphRag.Config.Models;

/// <summary>
/// Configuration for global map-reduce search.
/// </summary>
public sealed record GlobalSearchConfig
{
    /// <summary>Gets the map prompt template.</summary>
    public string? MapPrompt { get; init; }

    /// <summary>Gets the reduce prompt template.</summary>
    public string? ReducePrompt { get; init; }

    /// <summary>Gets the knowledge prompt template.</summary>
    public string? KnowledgePrompt { get; init; }

    /// <summary>Gets the completion model identifier.</summary>
    public string CompletionModelId { get; init; } = DefaultValues.DefaultCompletionModelId;

    /// <summary>Gets the maximum number of context tokens.</summary>
    public int MaxContextTokens { get; init; } = 12_000;

    /// <summary>Gets the maximum number of data tokens.</summary>
    public int DataMaxTokens { get; init; } = 12_000;

    /// <summary>Gets the maximum length for map responses.</summary>
    public int MapMaxLength { get; init; } = 1000;

    /// <summary>Gets the maximum length for reduce responses.</summary>
    public int ReduceMaxLength { get; init; } = 2000;

    /// <summary>Gets the threshold for dynamic community selection.</summary>
    public int DynamicSearchThreshold { get; init; } = 1;

    /// <summary>Gets a value indicating whether to keep parent communities in dynamic search.</summary>
    public bool DynamicSearchKeepParent { get; init; }

    /// <summary>Gets the number of repeats for dynamic search.</summary>
    public int DynamicSearchNumRepeats { get; init; } = 1;

    /// <summary>Gets a value indicating whether to use summary in dynamic search.</summary>
    public bool DynamicSearchUseSummary { get; init; }

    /// <summary>Gets the maximum hierarchy level for dynamic search.</summary>
    public int DynamicSearchMaxLevel { get; init; } = 2;
}
