// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Config.Enums;

namespace GraphRag.Config.Models;

/// <summary>
/// Configuration for NLP-based graph extraction.
/// </summary>
public sealed record ExtractGraphNlpConfig
{
    /// <summary>Gets a value indicating whether to normalize edge weights.</summary>
    public bool NormalizeEdgeWeights { get; init; } = true;

    /// <summary>Gets the text analyzer configuration.</summary>
    public TextAnalyzerConfig TextAnalyzer { get; init; } = new();

    /// <summary>Gets the number of concurrent requests.</summary>
    public int ConcurrentRequests { get; init; } = 25;

    /// <summary>Gets the async execution mode.</summary>
    public string AsyncMode { get; init; } = AsyncType.Threaded;
}
