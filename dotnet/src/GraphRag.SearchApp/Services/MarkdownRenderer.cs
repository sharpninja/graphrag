// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using Markdig;
using Microsoft.AspNetCore.Components;

namespace GraphRag.SearchApp.Services;

/// <summary>
/// Renders Markdown text to HTML using Markdig.
/// </summary>
public class MarkdownRenderer
{
    private static readonly MarkdownPipeline Pipeline = new MarkdownPipelineBuilder()
        .UseAdvancedExtensions()
        .Build();

    /// <summary>
    /// Converts Markdown text to an HTML <see cref="MarkupString"/>.
    /// </summary>
    /// <param name="markdown">The Markdown text to render.</param>
    /// <returns>An HTML markup string.</returns>
    public static MarkupString RenderToHtml(string? markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
        {
            return new MarkupString(string.Empty);
        }

        var html = Markdown.ToHtml(markdown, Pipeline);
        return new MarkupString(html);
    }
}
