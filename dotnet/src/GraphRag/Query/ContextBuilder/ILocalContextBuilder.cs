// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Query.ContextBuilder;

/// <summary>
/// Interface for building context used in local search operations.
/// </summary>
public interface ILocalContextBuilder
{
    /// <summary>
    /// Builds context for a local search query.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="history">Optional conversation history.</param>
    /// <returns>The context builder result.</returns>
    ContextBuilderResult BuildContext(string query, ConversationHistory? history);
}
