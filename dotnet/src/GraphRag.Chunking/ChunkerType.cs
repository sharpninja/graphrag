// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Chunking;

/// <summary>
/// Builtin chunker implementation types.
/// </summary>
public static class ChunkerType
{
    /// <summary>Gets the identifier for token-based chunking.</summary>
    public const string Tokens = "tokens";

    /// <summary>Gets the identifier for sentence-based chunking.</summary>
    public const string Sentence = "sentence";
}
