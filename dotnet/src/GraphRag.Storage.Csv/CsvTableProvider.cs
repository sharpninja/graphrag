// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Globalization;

using CsvHelper;
using CsvHelper.Configuration;

using GraphRag.Common.Discovery;
using GraphRag.Storage.Tables;

namespace GraphRag.Storage.Csv;

/// <summary>
/// CSV file-based implementation of <see cref="ITableProvider"/>.
/// </summary>
[StrategyImplementation("csv", typeof(ITableProvider))]
public sealed class CsvTableProvider : ITableProvider
{
    private readonly string _baseDir;

    /// <summary>
    /// Initializes a new instance of the <see cref="CsvTableProvider"/> class.
    /// </summary>
    /// <param name="baseDir">The base directory containing CSV files.</param>
    public CsvTableProvider(string baseDir)
    {
        ArgumentException.ThrowIfNullOrEmpty(baseDir);
        _baseDir = Path.GetFullPath(baseDir);
        Directory.CreateDirectory(_baseDir);
    }

    /// <inheritdoc/>
    public async Task<List<Dictionary<string, object?>>> ReadAsync(string tableName, CancellationToken cancellationToken = default)
    {
        var filePath = GetFilePath(tableName);
        if (!File.Exists(filePath))
        {
            return [];
        }

        using var reader = new StreamReader(filePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        });

        var rows = new List<Dictionary<string, object?>>();
        await foreach (var record in csv.GetRecordsAsync<dynamic>(cancellationToken))
        {
            var dict = new Dictionary<string, object?>();
            foreach (var property in (IDictionary<string, object>)record)
            {
                dict[property.Key] = property.Value;
            }

            rows.Add(dict);
        }

        return rows;
    }

    /// <inheritdoc/>
    public async Task WriteAsync(string tableName, List<Dictionary<string, object?>> rows, CancellationToken cancellationToken = default)
    {
        if (rows.Count == 0)
        {
            return;
        }

        var filePath = GetFilePath(tableName);
        using var writer = new StreamWriter(filePath);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));

        var headers = rows[0].Keys.ToList();
        foreach (var header in headers)
        {
            csv.WriteField(header);
        }

        await csv.NextRecordAsync().ConfigureAwait(false);

        foreach (var row in rows)
        {
            foreach (var header in headers)
            {
                row.TryGetValue(header, out var value);
                csv.WriteField(value?.ToString() ?? string.Empty);
            }

            await csv.NextRecordAsync().ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public Task<bool> HasAsync(string tableName, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(File.Exists(GetFilePath(tableName)));
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> List()
    {
        if (!Directory.Exists(_baseDir))
        {
            return [];
        }

        return Directory.EnumerateFiles(_baseDir, "*.csv")
            .Select(f => Path.GetFileNameWithoutExtension(f))
            .ToList();
    }

    /// <inheritdoc/>
    public ITable Open(string tableName, Func<Dictionary<string, object?>, object>? transformer = null, bool truncate = true)
    {
        return new CsvTable(GetFilePath(tableName), tableName, transformer, truncate);
    }

    private string GetFilePath(string tableName)
    {
        return Path.Combine(_baseDir, $"{tableName}.csv");
    }
}
