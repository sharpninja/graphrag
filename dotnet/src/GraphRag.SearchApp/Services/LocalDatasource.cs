// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Storage.Tables;

namespace GraphRag.SearchApp.Services;

/// <summary>
/// Reads data from a local file system dataset via an <see cref="ITableProvider"/>.
/// </summary>
public class LocalDatasource : IDatasource
{
    private readonly ITableProvider _tableProvider;
    private readonly string _basePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalDatasource"/> class.
    /// </summary>
    /// <param name="tableProvider">The table provider for reading parquet/CSV files.</param>
    /// <param name="basePath">The base path to the dataset output directory.</param>
    public LocalDatasource(ITableProvider tableProvider, string basePath)
    {
        ArgumentNullException.ThrowIfNull(tableProvider);
        _tableProvider = tableProvider;
        _basePath = basePath;
    }

    /// <inheritdoc />
    public async Task<List<Dictionary<string, object?>>> ReadTableAsync(
        string tableName,
        CancellationToken cancellationToken = default)
    {
        return await _tableProvider.ReadAsync(tableName, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task<string?> ReadSettingsAsync(
        string fileName,
        CancellationToken cancellationToken = default)
    {
        var fullPath = Path.Combine(_basePath, fileName);
        if (!File.Exists(fullPath))
        {
            return Task.FromResult<string?>(null);
        }

        return File.ReadAllTextAsync(fullPath, cancellationToken)!;
    }

    /// <inheritdoc />
    public Task<bool> HasTableAsync(
        string tableName,
        CancellationToken cancellationToken = default)
    {
        return _tableProvider.HasAsync(tableName, cancellationToken);
    }
}
