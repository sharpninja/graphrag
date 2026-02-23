// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Chunking;

namespace GraphRag.Tests.Unit.Chunking;

/// <summary>
/// Unit tests for <see cref="SentenceChunker"/>.
/// </summary>
public class SentenceChunkerTests
{
    [Fact]
    public void Chunk_MultipleSentences_SplitCorrectly()
    {
        var chunker = new SentenceChunker(Encode);

        var result = chunker.Chunk("Hello world. How are you? I am fine!");

        result.Should().HaveCount(3);
        result[0].Original.Should().Be("Hello world.");
        result[1].Original.Should().Be("How are you?");
        result[2].Original.Should().Be("I am fine!");
    }

    [Fact]
    public void Chunk_SingleSentence_ReturnsSingle()
    {
        var chunker = new SentenceChunker(Encode);

        var result = chunker.Chunk("Just one sentence");

        result.Should().HaveCount(1);
        result[0].Original.Should().Be("Just one sentence");
    }

    [Fact]
    public void Chunk_WithTransform_TransformApplied()
    {
        var chunker = new SentenceChunker(Encode);

        var result = chunker.Chunk("Hello world.", text => text.ToUpperInvariant());

        result.Should().HaveCount(1);
        result[0].Text.Should().Be("HELLO WORLD.");
        result[0].Original.Should().Be("Hello world.");
    }

    private static IReadOnlyList<int> Encode(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return Array.Empty<int>();
        }

        return Enumerable.Range(0, text.Split(' ').Length).ToList();
    }
}
