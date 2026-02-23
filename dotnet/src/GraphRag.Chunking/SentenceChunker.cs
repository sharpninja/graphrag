// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Text.RegularExpressions;

namespace GraphRag.Chunking;

/// <summary>
/// A chunker that splits text into chunks based on sentence boundaries.
/// </summary>
public partial class SentenceChunker : IChunker
{
    private readonly Func<string, IReadOnlyList<int>>? _encode;

    /// <summary>
    /// Initializes a new instance of the <see cref="SentenceChunker"/> class.
    /// </summary>
    /// <param name="encode">An optional encoding function used to compute token counts.</param>
    public SentenceChunker(Func<string, IReadOnlyList<int>>? encode = null)
    {
        _encode = encode;
    }

    /// <inheritdoc/>
    public List<TextChunk> Chunk(string text, Func<string, string>? transform = null)
    {
        var sentences = SplitSentences(text);
        return ChunkResultHelper.CreateChunkResults(sentences, transform, _encode);
    }

    /// <summary>
    /// Split text into sentences using a basic regex approximation of sentence boundaries.
    /// </summary>
    private static List<string> SplitSentences(string text)
    {
        // Split on sentence-ending punctuation followed by a space, or on newlines.
        var parts = SentenceSplitRegex().Split(text);
        var sentences = new List<string>();

        foreach (var part in parts)
        {
            if (!string.IsNullOrEmpty(part))
            {
                sentences.Add(part);
            }
        }

        return sentences;
    }

    [GeneratedRegex(@"(?<=[\.\!\?])\s+|\n+", RegexOptions.Compiled)]
    private static partial Regex SentenceSplitRegex();
}
