// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.SearchApp.Config;

namespace GraphRag.Tests.Unit.SearchApp.Config;

/// <summary>
/// Tests for <see cref="SearchAppConfig"/>.
/// </summary>
public class SearchAppConfigTests
{
    [Fact]
    public void Defaults_AreCorrect()
    {
        var config = new SearchAppConfig();

        config.DataRoot.Should().Be("./output");
        config.BlobAccountName.Should().BeEmpty();
        config.BlobContainerName.Should().BeEmpty();
        config.ListingFile.Should().Be("listing.json");
        config.DefaultSuggestedQuestions.Should().Be(5);
        config.CacheTtlSeconds.Should().Be(604800);
    }

    [Fact]
    public void Properties_CanBeSet()
    {
        var config = new SearchAppConfig
        {
            DataRoot = "/data",
            BlobAccountName = "myaccount",
            BlobContainerName = "mycontainer",
            ListingFile = "datasets.json",
            DefaultSuggestedQuestions = 10,
            CacheTtlSeconds = 3600,
        };

        config.DataRoot.Should().Be("/data");
        config.BlobAccountName.Should().Be("myaccount");
        config.BlobContainerName.Should().Be("mycontainer");
        config.ListingFile.Should().Be("datasets.json");
        config.DefaultSuggestedQuestions.Should().Be(10);
        config.CacheTtlSeconds.Should().Be(3600);
    }
}
