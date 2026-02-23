// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Index.Operations;

/// <summary>
/// Summarizes a collection of descriptions into a single summary.
/// </summary>
public interface ISummarizeExtractor
{
    /// <summary>
    /// Summarizes the given descriptions.
    /// </summary>
    /// <param name="descriptions">The descriptions to summarize.</param>
    /// <param name="ct">A token to cancel the operation.</param>
    /// <returns>A summarized string.</returns>
    Task<string> SummarizeAsync(IReadOnlyList<string> descriptions, CancellationToken ct);
}
