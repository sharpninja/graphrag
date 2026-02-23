// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Config.Defaults;

namespace GraphRag.Config.Models;

/// <summary>
/// Configuration for text chunking within the GraphRag pipeline.
/// </summary>
public sealed record GraphRagChunkingConfig
{
    /// <summary>Gets the chunker type to use.</summary>
    public string Type { get; init; } = "tokens";

    /// <summary>Gets the maximum number of tokens per chunk.</summary>
    public int Size { get; init; } = 1200;

    /// <summary>Gets the number of overlapping tokens between consecutive chunks.</summary>
    public int Overlap { get; init; } = 100;

    /// <summary>Gets the encoding model name used for tokenization.</summary>
    public string EncodingModel { get; init; } = DefaultValues.EncodingModel;

    /// <summary>Gets the list of metadata keys to prepend to each chunk.</summary>
    public IReadOnlyList<string>? PrependMetadata { get; init; }
}
