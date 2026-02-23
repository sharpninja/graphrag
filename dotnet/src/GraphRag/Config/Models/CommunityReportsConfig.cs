// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Config.Defaults;

namespace GraphRag.Config.Models;

/// <summary>
/// Configuration for community report generation.
/// </summary>
public sealed record CommunityReportsConfig
{
    /// <summary>Gets the completion model identifier.</summary>
    public string CompletionModelId { get; init; } = DefaultValues.DefaultCompletionModelId;

    /// <summary>Gets the model instance name for this workflow step.</summary>
    public string ModelInstanceName { get; init; } = "community_reporting";

    /// <summary>Gets the prompt template for graph-based community reports.</summary>
    public string? GraphPrompt { get; init; }

    /// <summary>Gets the prompt template for text-based community reports.</summary>
    public string? TextPrompt { get; init; }

    /// <summary>Gets the maximum output length in tokens.</summary>
    public int MaxLength { get; init; } = 2000;

    /// <summary>Gets the maximum input length in tokens.</summary>
    public int MaxInputLength { get; init; } = 8000;
}
