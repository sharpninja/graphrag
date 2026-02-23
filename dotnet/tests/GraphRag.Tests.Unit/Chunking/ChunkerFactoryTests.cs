// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Chunking;

namespace GraphRag.Tests.Unit.Chunking;

/// <summary>
/// Unit tests for <see cref="ChunkerFactory"/>.
/// </summary>
public class ChunkerFactoryTests
{
    [Fact]
    public void CreateChunker_TokenType_CreatesTokenChunker()
    {
        var factory = new ChunkerFactory();
        var config = new ChunkingConfig { Type = ChunkerType.Tokens };

        var chunker = factory.CreateChunker(config, Encode, Decode);

        chunker.Should().BeOfType<TokenChunker>();
    }

    [Fact]
    public void CreateChunker_SentenceType_CreatesSentenceChunker()
    {
        var factory = new ChunkerFactory();
        var config = new ChunkingConfig { Type = ChunkerType.Sentence };

        var chunker = factory.CreateChunker(config, Encode, Decode);

        chunker.Should().BeOfType<SentenceChunker>();
    }

    [Fact]
    public void CreateChunker_UnknownType_Throws()
    {
        var factory = new ChunkerFactory();
        var config = new ChunkingConfig { Type = "unknown" };

        var act = () => factory.CreateChunker(config, Encode, Decode);

        act.Should().Throw<InvalidOperationException>();
    }

    private static IReadOnlyList<int> Encode(string text) =>
        string.IsNullOrEmpty(text)
            ? Array.Empty<int>()
            : Enumerable.Range(0, text.Split(' ').Length).ToList();

    private static string Decode(IReadOnlyList<int> tokens) =>
        string.Join(' ', tokens.Select(t => t.ToString(System.Globalization.CultureInfo.InvariantCulture)));
}
