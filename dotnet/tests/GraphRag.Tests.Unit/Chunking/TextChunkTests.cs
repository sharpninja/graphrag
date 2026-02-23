// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Chunking;

namespace GraphRag.Tests.Unit.Chunking;

/// <summary>
/// Unit tests for <see cref="TextChunk"/>.
/// </summary>
public class TextChunkTests
{
    [Fact]
    public void Record_Equality_WorksCorrectly()
    {
        var chunk1 = new TextChunk("hello", "hello", 0, 0, 5, 1);
        var chunk2 = new TextChunk("hello", "hello", 0, 0, 5, 1);

        chunk1.Should().Be(chunk2);
        (chunk1 == chunk2).Should().BeTrue();
    }

    [Fact]
    public void Record_Properties_SetCorrectly()
    {
        var chunk = new TextChunk(
            Original: "original text",
            Text: "transformed text",
            Index: 2,
            StartChar: 10,
            EndChar: 23,
            TokenCount: 3);

        chunk.Original.Should().Be("original text");
        chunk.Text.Should().Be("transformed text");
        chunk.Index.Should().Be(2);
        chunk.StartChar.Should().Be(10);
        chunk.EndChar.Should().Be(23);
        chunk.TokenCount.Should().Be(3);
    }
}
