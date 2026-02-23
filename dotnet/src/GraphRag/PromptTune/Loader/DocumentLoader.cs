// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Config.Models;

namespace GraphRag.PromptTune.Loader;

/// <summary>
/// Loads and chunks documents for prompt tuning.
/// </summary>
public static class DocumentLoader
{
    /// <summary>
    /// Loads documents from the configured input source and splits them into chunks.
    /// </summary>
    /// <param name="config">The GraphRag configuration.</param>
    /// <param name="selectMethod">The document selection strategy (see <see cref="DocSelectionType"/>).</param>
    /// <param name="limit">The maximum number of chunks to return.</param>
    /// <param name="ct">A token to cancel the operation.</param>
    /// <returns>A read-only list of document chunks.</returns>
    public static Task<IReadOnlyList<string>> LoadDocsInChunksAsync(
        GraphRagConfig config,
        string selectMethod,
        int limit,
        CancellationToken ct)
    {
        // TODO: Load documents from input storage, chunk them, and apply selection strategy.
        return Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());
    }
}
