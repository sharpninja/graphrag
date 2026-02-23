// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Query.ContextBuilder;

/// <summary>
/// Interface for building context used in global search operations.
/// </summary>
public interface IGlobalContextBuilder
{
    /// <summary>
    /// Builds context asynchronously for a global search query.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="history">Optional conversation history.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The context builder result.</returns>
    Task<ContextBuilderResult> BuildContextAsync(
        string query,
        ConversationHistory? history,
        CancellationToken cancellationToken = default);
}
