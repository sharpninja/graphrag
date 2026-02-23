// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Storage.Tables;

namespace GraphRag.Query;

/// <summary>
/// Provides helper methods to load query data from an <see cref="ITableProvider"/>.
/// </summary>
public static class IndexerAdapters
{
    /// <summary>
    /// Reads entities from the table provider.
    /// </summary>
    /// <param name="tableProvider">The table provider to read from.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of entity records.</returns>
    public static async Task<List<Dictionary<string, object?>>> ReadEntitiesAsync(
        ITableProvider tableProvider,
        CancellationToken cancellationToken = default)
    {
        if (await tableProvider.HasAsync("entities", cancellationToken).ConfigureAwait(false))
        {
            return await tableProvider.ReadAsync("entities", cancellationToken).ConfigureAwait(false);
        }

        return [];
    }

    /// <summary>
    /// Reads relationships from the table provider.
    /// </summary>
    /// <param name="tableProvider">The table provider to read from.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of relationship records.</returns>
    public static async Task<List<Dictionary<string, object?>>> ReadRelationshipsAsync(
        ITableProvider tableProvider,
        CancellationToken cancellationToken = default)
    {
        if (await tableProvider.HasAsync("relationships", cancellationToken).ConfigureAwait(false))
        {
            return await tableProvider.ReadAsync("relationships", cancellationToken).ConfigureAwait(false);
        }

        return [];
    }

    /// <summary>
    /// Reads community reports from the table provider.
    /// </summary>
    /// <param name="tableProvider">The table provider to read from.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of community report records.</returns>
    public static async Task<List<Dictionary<string, object?>>> ReadCommunityReportsAsync(
        ITableProvider tableProvider,
        CancellationToken cancellationToken = default)
    {
        if (await tableProvider.HasAsync("community_reports", cancellationToken).ConfigureAwait(false))
        {
            return await tableProvider.ReadAsync("community_reports", cancellationToken).ConfigureAwait(false);
        }

        return [];
    }

    /// <summary>
    /// Reads text units from the table provider.
    /// </summary>
    /// <param name="tableProvider">The table provider to read from.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of text unit records.</returns>
    public static async Task<List<Dictionary<string, object?>>> ReadTextUnitsAsync(
        ITableProvider tableProvider,
        CancellationToken cancellationToken = default)
    {
        if (await tableProvider.HasAsync("text_units", cancellationToken).ConfigureAwait(false))
        {
            return await tableProvider.ReadAsync("text_units", cancellationToken).ConfigureAwait(false);
        }

        return [];
    }
}
