// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Config.Models;

namespace GraphRag.Tests.Unit.Config;

public class ResolvedPromptsTests
{
    [Fact]
    public void CommunityReportsConfig_ResolvedPrompts_FallsBackToEmbeddedResource()
    {
        var config = new CommunityReportsConfig();

        var (graphPrompt, textPrompt) = config.ResolvedPrompts();

        graphPrompt.Should().NotBeNullOrEmpty();
        textPrompt.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void CommunityReportsConfig_ResolvedPrompts_UsesConfiguredValues()
    {
        var config = new CommunityReportsConfig
        {
            GraphPrompt = "custom graph prompt",
            TextPrompt = "custom text prompt",
        };

        var (graphPrompt, textPrompt) = config.ResolvedPrompts();

        graphPrompt.Should().Be("custom graph prompt");
        textPrompt.Should().Be("custom text prompt");
    }

    [Fact]
    public void ExtractClaimsConfig_ResolvedPrompt_FallsBackToEmbeddedResource()
    {
        var config = new ExtractClaimsConfig();

        var prompt = config.ResolvedPrompt();

        prompt.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ExtractClaimsConfig_ResolvedPrompt_UsesConfiguredValue()
    {
        var config = new ExtractClaimsConfig { Prompt = "custom claims" };

        config.ResolvedPrompt().Should().Be("custom claims");
    }

    [Fact]
    public void ExtractGraphConfig_ResolvedPrompt_FallsBackToEmbeddedResource()
    {
        var config = new ExtractGraphConfig();

        var prompt = config.ResolvedPrompt();

        prompt.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void ExtractGraphConfig_ResolvedPrompt_UsesConfiguredValue()
    {
        var config = new ExtractGraphConfig { Prompt = "custom extract" };

        config.ResolvedPrompt().Should().Be("custom extract");
    }

    [Fact]
    public void SummarizeDescriptionsConfig_ResolvedPrompt_FallsBackToEmbeddedResource()
    {
        var config = new SummarizeDescriptionsConfig();

        var prompt = config.ResolvedPrompt();

        prompt.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void SummarizeDescriptionsConfig_ResolvedPrompt_UsesConfiguredValue()
    {
        var config = new SummarizeDescriptionsConfig { Prompt = "custom summarize" };

        config.ResolvedPrompt().Should().Be("custom summarize");
    }
}
