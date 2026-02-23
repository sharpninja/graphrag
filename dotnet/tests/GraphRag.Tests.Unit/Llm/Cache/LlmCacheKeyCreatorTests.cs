// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Llm.Cache;
using GraphRag.Llm.Types;

namespace GraphRag.Tests.Unit.Llm.Cache;

/// <summary>
/// Unit tests for <see cref="LlmCacheKeyCreator"/>.
/// </summary>
public class LlmCacheKeyCreatorTests
{
    [Fact]
    public void CreateKey_SameInputs_SameKey()
    {
        var args = new LlmCompletionArgs(Messages: [new LlmMessage("user", "hello")]);

        var key1 = LlmCacheKeyCreator.CreateKey(args, "gpt-4");
        var key2 = LlmCacheKeyCreator.CreateKey(args, "gpt-4");

        key1.Should().Be(key2);
    }

    [Fact]
    public void CreateKey_DifferentInputs_DifferentKey()
    {
        var args1 = new LlmCompletionArgs(Messages: [new LlmMessage("user", "hello")]);
        var args2 = new LlmCompletionArgs(Messages: [new LlmMessage("user", "goodbye")]);

        var key1 = LlmCacheKeyCreator.CreateKey(args1, "gpt-4");
        var key2 = LlmCacheKeyCreator.CreateKey(args2, "gpt-4");

        key1.Should().NotBe(key2);
    }
}
