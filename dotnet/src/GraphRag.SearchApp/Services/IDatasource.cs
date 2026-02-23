// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.SearchApp.Services;

/// <summary>
/// Interface for reading table data from a dataset source.
/// </summary>
public interface IDatasource
{
    /// <summary>
    /// Reads a table as a list of row dictionaries.
    /// </summary>
    /// <param name="tableName">The name of the table to read.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of dictionaries, each representing a row.</returns>
    Task<List<Dictionary<string, object?>>> ReadTableAsync(
        string tableName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads a settings file from the dataset.
    /// </summary>
    /// <param name="fileName">The name of the settings file.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The file content as a string, or null if not found.</returns>
    Task<string?> ReadSettingsAsync(
        string fileName,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks whether a table exists in the dataset.
    /// </summary>
    /// <param name="tableName">The name of the table to check.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns><c>true</c> if the table exists; otherwise <c>false</c>.</returns>
    Task<bool> HasTableAsync(
        string tableName,
        CancellationToken cancellationToken = default);
}
