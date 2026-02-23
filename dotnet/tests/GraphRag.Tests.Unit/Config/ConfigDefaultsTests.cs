// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Config.Defaults;
using GraphRag.Config.Enums;
using GraphRag.Config.Models;

namespace GraphRag.Tests.Unit.Config;

public class ConfigDefaultsTests
{
    [Fact]
    public void DefaultValues_Constants_AreCorrect()
    {
        DefaultValues.DefaultInputBaseDir.Should().Be("input");
        DefaultValues.DefaultOutputBaseDir.Should().Be("output");
        DefaultValues.DefaultCompletionModel.Should().Be("gpt-4.1");
        DefaultValues.DefaultEmbeddingModel.Should().Be("text-embedding-3-large");
        DefaultValues.DefaultModelProvider.Should().Be("openai");
    }

    [Fact]
    public void GraphRagConfig_DefaultAsyncMode_IsThreaded()
    {
        var config = new GraphRagConfig();

        config.AsyncMode.Should().Be(AsyncType.Threaded);
    }

    [Fact]
    public void GraphRagConfig_DefaultConcurrentRequests_Is25()
    {
        var config = new GraphRagConfig();

        config.ConcurrentRequests.Should().Be(25);
    }
}
