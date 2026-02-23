// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Query;

namespace GraphRag.Tests.Unit.Query;

/// <summary>
/// Unit tests for <see cref="SearchResult"/>.
/// </summary>
public class SearchResultTests
{
    [Fact]
    public void Record_Properties_SetCorrectly()
    {
        var contextData = new { Key = "value" };
        var result = new SearchResult(
            Response: "answer",
            ContextData: contextData,
            ContextText: "context",
            CompletionTime: 1.5,
            LlmCalls: 3,
            PromptTokens: 100,
            OutputTokens: 50);

        result.Response.Should().Be("answer");
        result.ContextData.Should().BeSameAs(contextData);
        result.ContextText.Should().Be("context");
        result.CompletionTime.Should().Be(1.5);
        result.LlmCalls.Should().Be(3);
        result.PromptTokens.Should().Be(100);
        result.OutputTokens.Should().Be(50);
    }
}
