// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Query.ContextBuilder;

/// <summary>
/// Interface for building context used in basic search operations.
/// </summary>
public interface IBasicContextBuilder
{
    /// <summary>
    /// Builds context for a basic search query.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="history">Optional conversation history.</param>
    /// <returns>The context builder result.</returns>
    ContextBuilderResult BuildContext(string query, ConversationHistory? history);
}
