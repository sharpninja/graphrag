// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Llm.Completion;
using GraphRag.Llm.Tokenizer;
using GraphRag.Llm.Types;

namespace GraphRag.Tests.Unit.Llm.Completion;

/// <summary>
/// Unit tests for <see cref="MockLlmCompletion"/>.
/// </summary>
public class MockLlmCompletionTests
{
    private readonly SimpleTokenizer _tokenizer = new();

    [Fact]
    public async Task CompleteAsync_ReturnsFirstMockResponse()
    {
        var sut = new MockLlmCompletion(["hello", "world"], _tokenizer);
        var args = new LlmCompletionArgs(Messages: [new LlmMessage("user", "test")]);

        var result = await sut.CompleteAsync(args);

        result.Content.Should().Be("hello");
    }

    [Fact]
    public async Task CompleteAsync_CyclesThroughResponses()
    {
        var sut = new MockLlmCompletion(["a", "b"], _tokenizer);
        var args = new LlmCompletionArgs(Messages: [new LlmMessage("user", "test")]);

        var r1 = await sut.CompleteAsync(args);
        var r2 = await sut.CompleteAsync(args);
        var r3 = await sut.CompleteAsync(args);

        r1.Content.Should().Be("a");
        r2.Content.Should().Be("b");
        r3.Content.Should().Be("a");
    }

    [Fact]
    public void Complete_ReturnsSyncResponse()
    {
        var sut = new MockLlmCompletion(["sync response"], _tokenizer);
        var args = new LlmCompletionArgs(Messages: [new LlmMessage("user", "hi")]);

        var result = sut.Complete(args);

        result.Content.Should().Be("sync response");
        result.Usage.Should().NotBeNull();
        result.Usage!.PromptTokens.Should().Be(1);
        result.Usage!.CompletionTokens.Should().Be(2);
    }
}
