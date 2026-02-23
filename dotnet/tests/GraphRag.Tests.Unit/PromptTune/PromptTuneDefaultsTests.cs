// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.PromptTune;

namespace GraphRag.Tests.Unit.PromptTune;

/// <summary>
/// Tests for <see cref="PromptTuneDefaults"/> constant values.
/// </summary>
public class PromptTuneDefaultsTests
{
    [Fact]
    public void Defaults_K_Is15()
    {
        PromptTuneDefaults.K.Should().Be(15);
    }

    [Fact]
    public void Defaults_Limit_Is15()
    {
        PromptTuneDefaults.Limit.Should().Be(15);
    }
}
