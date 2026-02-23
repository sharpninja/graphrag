// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Chunking;

/// <summary>
/// Provides an interface for splitting text into chunks.
/// </summary>
public interface IChunker
{
    /// <summary>
    /// Split the given text into a list of chunks.
    /// </summary>
    /// <param name="text">The text to chunk.</param>
    /// <param name="transform">An optional transformation function applied to each chunk's text.</param>
    /// <returns>A list of <see cref="TextChunk"/> instances.</returns>
    List<TextChunk> Chunk(string text, Func<string, string>? transform = null);
}
