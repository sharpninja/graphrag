// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Config.Defaults;

namespace GraphRag.Config.Models;

/// <summary>
/// Configuration for graph entity and relationship extraction.
/// </summary>
public sealed record ExtractGraphConfig
{
    /// <summary>Gets the completion model identifier.</summary>
    public string CompletionModelId { get; init; } = DefaultValues.DefaultCompletionModelId;

    /// <summary>Gets the model instance name for this workflow step.</summary>
    public string ModelInstanceName { get; init; } = "extract_graph";

    /// <summary>Gets the prompt template for graph extraction.</summary>
    public string? Prompt { get; init; }

    /// <summary>Gets the entity types to extract.</summary>
    public IReadOnlyList<string> EntityTypes { get; init; } = DefaultValues.DefaultEntityTypes;

    /// <summary>Gets the maximum number of gleaning iterations.</summary>
    public int MaxGleanings { get; init; } = 1;
}
