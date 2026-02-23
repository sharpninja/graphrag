// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Input;

namespace GraphRag.Tests.Unit.Input;

/// <summary>
/// Unit tests for <see cref="TextDocument"/>.
/// </summary>
public class TextDocumentTests
{
    [Fact]
    public void Get_StandardField_ReturnsValue()
    {
        var doc = new TextDocument("id1", "some text", "My Title", "2025-01-01");

        doc.Get("id").Should().Be("id1");
        doc.Get("text").Should().Be("some text");
        doc.Get("title").Should().Be("My Title");
        doc.Get("creation_date").Should().Be("2025-01-01");
    }

    [Fact]
    public void Get_RawDataField_ReturnsValue()
    {
        var rawData = new Dictionary<string, object?> { ["custom_field"] = "custom_value" };
        var doc = new TextDocument("id1", "text", "title", "2025-01-01", rawData);

        doc.Get("custom_field").Should().Be("custom_value");
    }

    [Fact]
    public void Get_MissingField_ReturnsDefault()
    {
        var doc = new TextDocument("id1", "text", "title", "2025-01-01");

        doc.Get("nonexistent").Should().BeNull();
        doc.Get("nonexistent", "fallback").Should().Be("fallback");
    }

    [Fact]
    public void Collect_MultipleFields_ReturnsDict()
    {
        var rawData = new Dictionary<string, object?> { ["extra"] = 42 };
        var doc = new TextDocument("id1", "text", "title", "2025-01-01", rawData);

        var result = doc.Collect(Fields);

        result.Should().ContainKey("id").WhoseValue.Should().Be("id1");
        result.Should().ContainKey("title").WhoseValue.Should().Be("title");
        result.Should().ContainKey("extra").WhoseValue.Should().Be(42);
        result.Should().NotContainKey("missing");
    }

    private static readonly string[] Fields = ["id", "title", "extra", "missing"];
}
