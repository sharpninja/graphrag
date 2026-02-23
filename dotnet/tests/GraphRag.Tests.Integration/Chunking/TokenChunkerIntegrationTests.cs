// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;

using GraphRag.Chunking;

namespace GraphRag.Tests.Integration.Chunking;

public class TokenChunkerIntegrationTests
{
    [Fact]
    public void Chunk_LargeDocument_ProducesCorrectChunks()
    {
        // Arrange – build a realistic-size document (~320 words)
        var sentences = Enumerable.Range(1, 40)
            .Select(i => $"Sentence number {i} provides important context about the topic")
            .ToList();
        var document = string.Join(". ", sentences) + ".";
        var words = document.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        IReadOnlyList<int> Encode(string text)
        {
            var w = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return Enumerable.Range(0, w.Length).ToList();
        }

        string Decode(IReadOnlyList<int> tokens)
        {
            return string.Join(" ", tokens.Select(i => i < words.Length ? words[i] : string.Empty));
        }

        const int chunkSize = 50;
        const int overlap = 10;
        var chunker = new TokenChunker(chunkSize, overlap, Encode, Decode);

        // Act
        var chunks = chunker.Chunk(document);

        // Assert
        chunks.Should().NotBeEmpty();
        chunks.Count.Should().BeGreaterThan(1, "a large document should produce multiple chunks");

        foreach (var chunk in chunks)
        {
            chunk.Text.Should().NotBeNullOrWhiteSpace();
        }

        // First chunk should start at index 0
        chunks[0].Index.Should().Be(0);
    }
}
