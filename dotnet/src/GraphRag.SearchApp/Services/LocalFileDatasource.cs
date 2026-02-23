// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.SearchApp.Services;

/// <summary>
/// A simple local file-based datasource that reads parquet files from disk.
/// </summary>
public class LocalFileDatasource : IDatasource
{
    private readonly string _basePath;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalFileDatasource"/> class.
    /// </summary>
    /// <param name="basePath">The base directory containing dataset output files.</param>
    public LocalFileDatasource(string basePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(basePath);
        _basePath = basePath;
    }

    /// <inheritdoc />
    public async Task<List<Dictionary<string, object?>>> ReadTableAsync(
        string tableName,
        CancellationToken cancellationToken = default)
    {
        var filePath = FindTableFile(tableName)
            ?? throw new FileNotFoundException($"Table '{tableName}' not found in '{_basePath}'.");

        using var stream = File.OpenRead(filePath);
        var rows = new List<Dictionary<string, object?>>();
        using var reader = await Parquet.ParquetReader.CreateAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);
        for (int g = 0; g < reader.RowGroupCount; g++)
        {
            using var groupReader = reader.OpenRowGroupReader(g);
            var fields = reader.Schema.GetDataFields();
            var columns = new Dictionary<string, object?[]>();
            foreach (var field in fields)
            {
                var column = await groupReader.ReadColumnAsync(field, cancellationToken).ConfigureAwait(false);
                columns[field.Name] = column.Data.Cast<object?>().ToArray();
            }

            var rowCount = columns.Values.First().Length;
            for (int r = 0; r < rowCount; r++)
            {
                var row = new Dictionary<string, object?>();
                foreach (var (name, data) in columns)
                {
                    row[name] = data[r];
                }

                rows.Add(row);
            }
        }

        return rows;
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
        var found = FindTableFile(tableName) is not null;
        return Task.FromResult(found);
    }

    private string? FindTableFile(string tableName)
    {
        // Try common extensions: .parquet, .csv
        string[] extensions = [".parquet", ".csv"];
        foreach (var ext in extensions)
        {
            var path = Path.Combine(_basePath, tableName + ext);
            if (File.Exists(path))
            {
                return path;
            }

            // Also check with "output/" prefix stripped
            var altPath = Path.Combine(_basePath, Path.GetFileName(tableName) + ext);
            if (File.Exists(altPath))
            {
                return altPath;
            }
        }

        return null;
    }
}
