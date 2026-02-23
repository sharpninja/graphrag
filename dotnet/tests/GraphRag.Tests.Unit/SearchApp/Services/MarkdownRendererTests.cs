// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.SearchApp.Services;

namespace GraphRag.Tests.Unit.SearchApp.Services;

/// <summary>
/// Tests for <see cref="MarkdownRenderer"/>.
/// </summary>
public class MarkdownRendererTests
{
    [Fact]
    public void RenderToHtml_NullInput_ReturnsEmpty()
    {
        var result = MarkdownRenderer.RenderToHtml(null);
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public void RenderToHtml_EmptyInput_ReturnsEmpty()
    {
        var result = MarkdownRenderer.RenderToHtml(string.Empty);
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public void RenderToHtml_SimpleMarkdown_ReturnsHtml()
    {
        var result = MarkdownRenderer.RenderToHtml("**bold** text");
        result.Value.Should().Contain("<strong>bold</strong>");
        result.Value.Should().Contain("text");
    }

    [Fact]
    public void RenderToHtml_HeaderMarkdown_ReturnsHtml()
    {
        var result = MarkdownRenderer.RenderToHtml("# Title");
        result.Value.Should().Contain("<h1");
        result.Value.Should().Contain("Title</h1>");
    }

    [Fact]
    public void RenderToHtml_ListMarkdown_ReturnsHtml()
    {
        var result = MarkdownRenderer.RenderToHtml("- item 1\n- item 2");
        result.Value.Should().Contain("<li>item 1</li>");
        result.Value.Should().Contain("<li>item 2</li>");
    }
}
