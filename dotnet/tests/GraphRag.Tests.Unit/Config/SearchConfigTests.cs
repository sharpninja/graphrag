// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Config.Models;

namespace GraphRag.Tests.Unit.Config;

public class SearchConfigTests
{
    [Fact]
    public void BasicSearchConfig_DefaultK_Is10()
    {
        var config = new BasicSearchConfig();

        config.K.Should().Be(10);
    }

    [Fact]
    public void LocalSearchConfig_DefaultTextUnitProp_Is0_5()
    {
        var config = new LocalSearchConfig();

        config.TextUnitProp.Should().Be(0.5);
    }

    [Fact]
    public void GlobalSearchConfig_DefaultMapMaxLength_Is1000()
    {
        var config = new GlobalSearchConfig();

        config.MapMaxLength.Should().Be(1000);
    }
}
