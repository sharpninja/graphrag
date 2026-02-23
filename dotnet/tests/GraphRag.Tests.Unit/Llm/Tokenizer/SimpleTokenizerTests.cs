// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Llm.Tokenizer;

namespace GraphRag.Tests.Unit.Llm.Tokenizer;

/// <summary>
/// Unit tests for <see cref="SimpleTokenizer"/>.
/// </summary>
public class SimpleTokenizerTests
{
    private readonly SimpleTokenizer _sut = new();

    [Fact]
    public void Encode_SplitsOnWhitespace()
    {
        var tokens = _sut.Encode("hello world foo");

        tokens.Should().HaveCount(3);
        tokens.Should().BeEquivalentTo([0, 1, 2]);
    }

    [Fact]
    public void Decode_JoinsWithSpaces()
    {
        var result = _sut.Decode([0, 1, 2]);

        result.Should().Be("0 1 2");
    }

    [Fact]
    public void CountTokens_ReturnsCorrectCount()
    {
        var count = _sut.CountTokens("one two three four");

        count.Should().Be(4);
    }

    [Fact]
    public void Encode_EmptyString_ReturnsEmpty()
    {
        var tokens = _sut.Encode(string.Empty);

        tokens.Should().BeEmpty();
    }
}
