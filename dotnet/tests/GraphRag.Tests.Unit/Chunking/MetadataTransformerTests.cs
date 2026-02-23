// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Chunking;

namespace GraphRag.Tests.Unit.Chunking;

/// <summary>
/// Unit tests for <see cref="MetadataTransformer"/>.
/// </summary>
public class MetadataTransformerTests
{
    [Fact]
    public void AddMetadata_Prepend_PrependsToText()
    {
        var metadata = new Dictionary<string, object?>
        {
            ["title"] = "Test",
            ["author"] = "Alice",
        };

        var transform = MetadataTransformer.AddMetadata(metadata);

        var result = transform("Hello world");

        result.Should().Be("title: Test\nauthor: Alice\nHello world");
    }

    [Fact]
    public void AddMetadata_Append_AppendsToText()
    {
        var metadata = new Dictionary<string, object?>
        {
            ["title"] = "Test",
        };

        var transform = MetadataTransformer.AddMetadata(metadata, append: true);

        var result = transform("Hello world");

        result.Should().Be("Hello world\ntitle: Test");
    }
}
