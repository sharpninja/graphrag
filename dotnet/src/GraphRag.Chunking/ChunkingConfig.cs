// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Chunking;

/// <summary>
/// The default configuration section for chunking.
/// </summary>
public sealed record ChunkingConfig
{
    /// <summary>Gets the chunker type to use. Builtin types include "tokens" and "sentence".</summary>
    public string Type { get; init; } = ChunkerType.Tokens;

    /// <summary>Gets the encoding model name used for tokenization.</summary>
    public string? EncodingModel { get; init; }

    /// <summary>Gets the maximum number of tokens per chunk.</summary>
    public int Size { get; init; } = 1200;

    /// <summary>Gets the number of overlapping tokens between consecutive chunks.</summary>
    public int Overlap { get; init; } = 100;

    /// <summary>Gets the list of metadata keys to prepend to each chunk.</summary>
    public IReadOnlyList<string>? PrependMetadata { get; init; }
}
