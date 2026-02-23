// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Config.Defaults;

namespace GraphRag.Config.Models;

/// <summary>
/// Configuration for claim extraction.
/// </summary>
public sealed record ExtractClaimsConfig
{
    /// <summary>Gets a value indicating whether claim extraction is enabled.</summary>
    public bool Enabled { get; init; }

    /// <summary>Gets the completion model identifier.</summary>
    public string CompletionModelId { get; init; } = DefaultValues.DefaultCompletionModelId;

    /// <summary>Gets the model instance name for this workflow step.</summary>
    public string ModelInstanceName { get; init; } = "extract_claims";

    /// <summary>Gets the prompt template for claim extraction.</summary>
    public string? Prompt { get; init; }

    /// <summary>Gets the description of the claims to extract.</summary>
    public string? Description { get; init; }

    /// <summary>Gets the maximum number of gleaning iterations.</summary>
    public int MaxGleanings { get; init; } = 1;
}
