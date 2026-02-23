// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Query;

/// <summary>
/// Represents the result of a global search operation, including intermediate map results.
/// </summary>
/// <param name="Response">The text response from the search.</param>
/// <param name="ContextData">Optional structured context data used in the search.</param>
/// <param name="ContextText">Optional text representation of the context.</param>
/// <param name="CompletionTime">The time in seconds taken to complete the search.</param>
/// <param name="LlmCalls">The number of LLM calls made during the search.</param>
/// <param name="PromptTokens">The total number of prompt tokens used.</param>
/// <param name="OutputTokens">The total number of output tokens generated.</param>
/// <param name="MapResults">Optional list of intermediate map search results.</param>
public sealed record GlobalSearchResult(
    string Response,
    object? ContextData = null,
    string? ContextText = null,
    double CompletionTime = 0,
    int LlmCalls = 0,
    int PromptTokens = 0,
    int OutputTokens = 0,
    IReadOnlyList<SearchResult>? MapResults = null) : SearchResult(Response, ContextData, ContextText, CompletionTime, LlmCalls, PromptTokens, OutputTokens);
