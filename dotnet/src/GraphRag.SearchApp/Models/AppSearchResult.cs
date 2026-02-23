// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.SearchApp.Models;

/// <summary>
/// Wraps a search result with its type and performance metrics.
/// </summary>
/// <param name="Type">The search type that produced this result.</param>
/// <param name="Response">The LLM-generated response text.</param>
/// <param name="CompletionTime">The time in seconds to complete the search.</param>
/// <param name="LlmCalls">The number of LLM calls made.</param>
/// <param name="PromptTokens">The total prompt tokens used.</param>
/// <param name="OutputTokens">The total output tokens generated.</param>
/// <param name="ContextData">Optional structured context data from the search.</param>
public sealed record AppSearchResult(
    SearchType Type,
    string Response,
    double CompletionTime = 0,
    int LlmCalls = 0,
    int PromptTokens = 0,
    int OutputTokens = 0,
    object? ContextData = null);
