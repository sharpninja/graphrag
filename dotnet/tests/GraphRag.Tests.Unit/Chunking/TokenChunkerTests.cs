// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Chunking;

namespace GraphRag.Tests.Unit.Chunking;

/// <summary>
/// Unit tests for <see cref="TokenChunker"/>.
/// </summary>
public class TokenChunkerTests
{
    [Fact]
    public void Chunk_ShortText_ReturnsSingleChunk()
    {
        var (chunker, _) = CreateWordChunker(size: 10);

        var result = chunker.Chunk("hello world");

        result.Should().HaveCount(1);
        result[0].Text.Should().Be("hello world");
        result[0].Index.Should().Be(0);
    }

    [Fact]
    public void Chunk_LongText_ReturnsMultipleChunks()
    {
        var (chunker, _) = CreateWordChunker(size: 3);

        var result = chunker.Chunk("one two three four five six");

        result.Should().HaveCount(2);
        result[0].Text.Should().Be("one two three");
        result[1].Text.Should().Be("four five six");
    }

    [Fact]
    public void Chunk_WithOverlap_ChunksOverlap()
    {
        var (chunker, _) = CreateWordChunker(size: 3, overlap: 1);

        var result = chunker.Chunk("one two three four five");

        result.Should().HaveCountGreaterThan(1);

        // With size=3 and overlap=1, step = 2:
        // Chunk 0: tokens 0-2 -> "one two three"
        // Chunk 1: tokens 2-4 -> "three four five"
        result[0].Text.Should().Be("one two three");
        result[1].Text.Should().Be("three four five");
    }

    [Fact]
    public void Chunk_WithTransform_TransformApplied()
    {
        var (chunker, _) = CreateWordChunker(size: 10);

        var result = chunker.Chunk("hello world", text => text.ToUpperInvariant());

        result.Should().HaveCount(1);
        result[0].Text.Should().Be("HELLO WORLD");
        result[0].Original.Should().Be("hello world");
    }

    [Fact]
    public void Chunk_EmptyText_ReturnsEmptyList()
    {
        var (chunker, _) = CreateWordChunker(size: 10);

        var result = chunker.Chunk(string.Empty);

        result.Should().BeEmpty();
    }

    /// <summary>
    /// Creates a TokenChunker that works with space-separated words as tokens.
    /// </summary>
    private static (TokenChunker Chunker, Func<string, IReadOnlyList<int>> Encode) CreateWordChunker(int size, int overlap = 0)
    {
        string[] capturedWords = Array.Empty<string>();

        IReadOnlyList<int> Encode(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return Array.Empty<int>();
            }

            capturedWords = text.Split(' ');
            return Enumerable.Range(0, capturedWords.Length).ToList();
        }

        string Decode(IReadOnlyList<int> tokens)
        {
            return string.Join(' ', tokens.Select(t => capturedWords[t]));
        }

        return (new TokenChunker(size, overlap, Encode, Decode), Encode);
    }
}
