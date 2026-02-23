// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Query;

/// <summary>
/// Interface for executing search operations against a knowledge graph.
/// </summary>
public interface ISearch
{
    /// <summary>
    /// Executes a search query asynchronously.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="history">Optional conversation history for multi-turn queries.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The search result.</returns>
    Task<SearchResult> SearchAsync(
        string query,
        ConversationHistory? history = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a search query and streams the response asynchronously.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="history">Optional conversation history for multi-turn queries.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An async enumerable of response text chunks.</returns>
    IAsyncEnumerable<string> StreamSearchAsync(
        string query,
        ConversationHistory? history = null,
        CancellationToken cancellationToken = default);
}
