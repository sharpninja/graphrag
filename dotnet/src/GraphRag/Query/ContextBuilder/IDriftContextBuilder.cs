// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Query.ContextBuilder;

/// <summary>
/// Interface for building context used in DRIFT search operations.
/// </summary>
public interface IDriftContextBuilder
{
    /// <summary>
    /// Builds context asynchronously for a DRIFT search query.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The context builder result.</returns>
    Task<ContextBuilderResult> BuildContextAsync(
        string query,
        CancellationToken cancellationToken = default);
}
