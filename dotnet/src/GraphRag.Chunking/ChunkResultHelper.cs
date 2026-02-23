// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Chunking;

/// <summary>
/// Helper methods for creating <see cref="TextChunk"/> results from raw chunk strings.
/// </summary>
public static class ChunkResultHelper
{
    /// <summary>
    /// Create a list of <see cref="TextChunk"/> instances from raw chunk strings,
    /// calculating character positions and optionally applying a transform and computing token counts.
    /// </summary>
    /// <param name="chunks">The raw text chunks.</param>
    /// <param name="transform">An optional transformation function applied to each chunk's text.</param>
    /// <param name="encode">An optional encoding function used to compute token counts.</param>
    /// <returns>A list of <see cref="TextChunk"/> instances.</returns>
    public static List<TextChunk> CreateChunkResults(
        List<string> chunks,
        Func<string, string>? transform,
        Func<string, IReadOnlyList<int>>? encode)
    {
        var results = new List<TextChunk>(chunks.Count);
        int charOffset = 0;

        for (int i = 0; i < chunks.Count; i++)
        {
            var original = chunks[i];
            var text = transform is not null ? transform(original) : original;
            int? tokenCount = encode is not null ? encode(text).Count : null;

            int startChar = charOffset;
            int endChar = charOffset + original.Length;

            results.Add(new TextChunk(
                Original: original,
                Text: text,
                Index: i,
                StartChar: startChar,
                EndChar: endChar,
                TokenCount: tokenCount));

            charOffset = endChar;
        }

        return results;
    }
}
