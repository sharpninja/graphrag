// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Llm.Embedding;
using GraphRag.Llm.Tokenizer;
using GraphRag.Llm.Types;

namespace GraphRag.Tests.Unit.Llm.Embedding;

/// <summary>
/// Unit tests for <see cref="MockLlmEmbedding"/>.
/// </summary>
public class MockLlmEmbeddingTests
{
    private readonly SimpleTokenizer _tokenizer = new();

    [Fact]
    public async Task EmbedAsync_ReturnsEmbeddingsWithCorrectDimensions()
    {
        var sut = new MockLlmEmbedding(dimensions: 8, tokenizer: _tokenizer);
        var args = new LlmEmbeddingArgs(Input: ["hello world"]);

        var result = await sut.EmbedAsync(args);

        result.Embeddings.Should().HaveCount(1);
        result.Embeddings[0].Should().HaveCount(8);
    }

    [Fact]
    public async Task EmbedAsync_MultipleInputs_ReturnsMultipleEmbeddings()
    {
        var sut = new MockLlmEmbedding(dimensions: 4, tokenizer: _tokenizer);
        var args = new LlmEmbeddingArgs(Input: ["one", "two", "three"]);

        var result = await sut.EmbedAsync(args);

        result.Embeddings.Should().HaveCount(3);
        result.Usage.Should().NotBeNull();
        result.Usage!.TotalTokens.Should().Be(3);
    }
}
