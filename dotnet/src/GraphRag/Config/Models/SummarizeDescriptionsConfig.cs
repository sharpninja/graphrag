// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Config.Defaults;

namespace GraphRag.Config.Models;

/// <summary>
/// Configuration for entity and relationship description summarization.
/// </summary>
public sealed record SummarizeDescriptionsConfig
{
    /// <summary>Gets the completion model identifier.</summary>
    public string CompletionModelId { get; init; } = DefaultValues.DefaultCompletionModelId;

    /// <summary>Gets the model instance name for this workflow step.</summary>
    public string ModelInstanceName { get; init; } = "summarize_descriptions";

    /// <summary>Gets the prompt template for summarization.</summary>
    public string? Prompt { get; init; }

    /// <summary>Gets the maximum output length in tokens.</summary>
    public int MaxLength { get; init; } = 500;

    /// <summary>Gets the maximum number of input tokens.</summary>
    public int MaxInputTokens { get; init; } = 4000;
}
