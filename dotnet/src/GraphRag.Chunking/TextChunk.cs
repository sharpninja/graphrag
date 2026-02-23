// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Chunking;

/// <summary>
/// Represents a single chunk of text extracted from a document.
/// </summary>
/// <param name="Original">The raw text before any transformation.</param>
/// <param name="Text">The final transformed content.</param>
/// <param name="Index">The zero-based position of this chunk in the document.</param>
/// <param name="StartChar">The character position where this chunk begins in the source text.</param>
/// <param name="EndChar">The character position where this chunk ends in the source text.</param>
/// <param name="TokenCount">The optional token count for this chunk.</param>
public sealed record TextChunk(
    string Original,
    string Text,
    int Index,
    int StartChar,
    int EndChar,
    int? TokenCount);
