// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Chunking;

/// <summary>
/// A chunker that splits text into chunks based on token counts.
/// </summary>
public class TokenChunker : IChunker
{
    private readonly int _size;
    private readonly int _overlap;
    private readonly Func<string, IReadOnlyList<int>> _encode;
    private readonly Func<IReadOnlyList<int>, string> _decode;

    /// <summary>
    /// Initializes a new instance of the <see cref="TokenChunker"/> class.
    /// </summary>
    /// <param name="size">The maximum number of tokens per chunk.</param>
    /// <param name="overlap">The number of overlapping tokens between consecutive chunks.</param>
    /// <param name="encode">A function that encodes text into a list of token IDs.</param>
    /// <param name="decode">A function that decodes a list of token IDs back into text.</param>
    public TokenChunker(
        int size,
        int overlap,
        Func<string, IReadOnlyList<int>> encode,
        Func<IReadOnlyList<int>, string> decode)
    {
        _size = size;
        _overlap = overlap;
        _encode = encode;
        _decode = decode;
    }

    /// <inheritdoc/>
    public List<TextChunk> Chunk(string text, Func<string, string>? transform = null)
    {
        var tokens = _encode(text);
        int total = tokens.Count;
        var chunks = new List<string>();

        int startIdx = 0;
        int curIdx = Math.Min(startIdx + _size, total);

        while (startIdx < total)
        {
            var tokenRange = tokens.Skip(startIdx).Take(curIdx - startIdx).ToList();
            var chunkText = _decode(tokenRange);
            chunks.Add(chunkText);

            startIdx += _size - _overlap;
            curIdx = Math.Min(startIdx + _size, total);
        }

        return ChunkResultHelper.CreateChunkResults(chunks, transform, _encode);
    }
}
