// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Storage.Tables;

/// <summary>
/// Provides a table-based storage interface supporting row-dictionary operations and streaming access.
/// </summary>
public interface ITableProvider
{
    /// <summary>
    /// Read the entire table as a list of row dictionaries.
    /// </summary>
    /// <param name="tableName">The name of the table to read.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of dictionaries, each representing a row.</returns>
    Task<List<Dictionary<string, object?>>> ReadAsync(string tableName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Write rows to a table.
    /// </summary>
    /// <param name="tableName">The name of the table to write.</param>
    /// <param name="rows">The rows to write, each represented as a dictionary.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task WriteAsync(string tableName, List<Dictionary<string, object?>> rows, CancellationToken cancellationToken = default);

    /// <summary>
    /// Check if a table exists in the provider.
    /// </summary>
    /// <param name="tableName">The name of the table to check.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns><c>true</c> if the table exists; otherwise, <c>false</c>.</returns>
    Task<bool> HasAsync(string tableName, CancellationToken cancellationToken = default);

    /// <summary>
    /// List all table names in the provider.
    /// </summary>
    /// <returns>A read-only list of table names without file extensions.</returns>
    IReadOnlyList<string> List();

    /// <summary>
    /// Open a table for row-by-row streaming operations.
    /// </summary>
    /// <param name="tableName">The name of the table to open.</param>
    /// <param name="transformer">Optional transformer function to apply to each row.</param>
    /// <param name="truncate">If <c>true</c> (default), truncate existing file on first write; otherwise append.</param>
    /// <returns>An <see cref="ITable"/> instance for streaming row operations.</returns>
    ITable Open(string tableName, Func<Dictionary<string, object?>, object>? transformer = null, bool truncate = true);
}
