// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Llm.Completion;
using GraphRag.Llm.Config;
using GraphRag.Llm.Factories;

namespace GraphRag.Tests.Unit.Llm.Factories;

/// <summary>
/// Unit tests for <see cref="CompletionFactory"/>.
/// </summary>
public class CompletionFactoryTests
{
    [Fact]
    public void CreateCompletion_MockType_CreatesInstance()
    {
        var sut = new CompletionFactory();
        var config = new ModelConfig { Type = LlmProviderType.MockLlm };

        var result = sut.CreateCompletion(config);

        result.Should().BeOfType<MockLlmCompletion>();
    }

    [Fact]
    public void CreateCompletion_UnknownType_Throws()
    {
        var sut = new CompletionFactory();
        var config = new ModelConfig { Type = "unknown_provider" };

        var act = () => sut.CreateCompletion(config);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*unknown_provider*not registered*");
    }
}
