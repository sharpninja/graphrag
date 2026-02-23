// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Storage.Tables;

/// <summary>
/// Provides streaming row-by-row access to table data with async iteration and write capabilities.
/// </summary>
public interface ITable : IAsyncDisposable
{
    /// <summary>
    /// Yield rows asynchronously, transformed if a transformer was provided.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An async enumerable of rows, either as dictionaries or transformed types.</returns>
    IAsyncEnumerable<object> GetRowsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Return the number of rows asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The number of rows in the table.</returns>
    Task<int> LengthAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a row with the given ID exists.
    /// </summary>
    /// <param name="rowId">The ID value to search for.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns><c>true</c> if a row with matching ID exists; otherwise, <c>false</c>.</returns>
    Task<bool> HasAsync(string rowId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Write a single row to the table.
    /// </summary>
    /// <param name="row">Dictionary representing a single row to write.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task WriteAsync(Dictionary<string, object?> row, CancellationToken cancellationToken = default);

    /// <summary>
    /// Flush buffered writes and release resources.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CloseAsync(CancellationToken cancellationToken = default);
}
