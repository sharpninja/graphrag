// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Globalization;

using CsvHelper;
using CsvHelper.Configuration;

using GraphRag.Storage.Tables;

namespace GraphRag.Storage.Csv;

/// <summary>
/// CSV file-based implementation of <see cref="ITable"/> for streaming row operations.
/// </summary>
public sealed class CsvTable : ITable
{
    private readonly string _filePath;
    private readonly string _name;
    private readonly Func<Dictionary<string, object?>, object>? _transformer;
    private readonly bool _truncate;
    private readonly List<Dictionary<string, object?>> _buffer = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="CsvTable"/> class.
    /// </summary>
    /// <param name="filePath">The path to the CSV file.</param>
    /// <param name="name">The table name.</param>
    /// <param name="transformer">Optional transformer function applied to each row during read.</param>
    /// <param name="truncate">Whether to truncate the file on first write.</param>
    internal CsvTable(string filePath, string name, Func<Dictionary<string, object?>, object>? transformer, bool truncate)
    {
        _filePath = filePath;
        _name = name;
        _transformer = transformer;
        _truncate = truncate;
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<object> GetRowsAsync([System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
        {
            yield break;
        }

        using var reader = new StreamReader(_filePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        });

        await foreach (var record in csv.GetRecordsAsync<dynamic>(cancellationToken))
        {
            var dict = new Dictionary<string, object?>();
            foreach (var property in (IDictionary<string, object>)record)
            {
                dict[property.Key] = property.Value;
            }

            yield return _transformer is not null ? _transformer(dict) : dict;
        }
    }

    /// <inheritdoc/>
    public async Task<int> LengthAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
        {
            return 0;
        }

        int count = 0;
        using var reader = new StreamReader(_filePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        });

        await foreach (var record in csv.GetRecordsAsync<dynamic>(cancellationToken))
        {
            _ = record;
            count++;
        }

        return count;
    }

    /// <inheritdoc/>
    public async Task<bool> HasAsync(string rowId, CancellationToken cancellationToken = default)
    {
        await foreach (var row in GetRowsAsync(cancellationToken))
        {
            if (row is Dictionary<string, object?> dict && dict.TryGetValue("id", out var id) && string.Equals(id?.ToString(), rowId, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
    }

    /// <inheritdoc/>
    public Task WriteAsync(Dictionary<string, object?> row, CancellationToken cancellationToken = default)
    {
        _buffer.Add(row);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task CloseAsync(CancellationToken cancellationToken = default)
    {
        if (_buffer.Count == 0)
        {
            return;
        }

        using var writer = new StreamWriter(_filePath, append: !_truncate);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));

        var headers = _buffer[0].Keys.ToList();
        foreach (var header in headers)
        {
            csv.WriteField(header);
        }

        await csv.NextRecordAsync().ConfigureAwait(false);

        foreach (var row in _buffer)
        {
            foreach (var header in headers)
            {
                row.TryGetValue(header, out var value);
                csv.WriteField(value?.ToString() ?? string.Empty);
            }

            await csv.NextRecordAsync().ConfigureAwait(false);
        }

        _buffer.Clear();
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await CloseAsync().ConfigureAwait(false);
    }
}
