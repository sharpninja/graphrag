// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Storage.Tables;

using Parquet;
using Parquet.Data;
using Parquet.Schema;

namespace GraphRag.Storage.Parquet;

/// <summary>
/// Parquet file-based implementation of <see cref="ITable"/> for streaming row operations.
/// </summary>
public sealed class ParquetTable : ITable
{
    private readonly string _filePath;
    private readonly string _name;
    private readonly Func<Dictionary<string, object?>, object>? _transformer;
    private readonly bool _truncate;
    private readonly List<Dictionary<string, object?>> _buffer = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="ParquetTable"/> class.
    /// </summary>
    /// <param name="filePath">The path to the Parquet file.</param>
    /// <param name="name">The table name.</param>
    /// <param name="transformer">Optional transformer function applied to each row during read.</param>
    /// <param name="truncate">Whether to truncate the file on first write.</param>
    internal ParquetTable(string filePath, string name, Func<Dictionary<string, object?>, object>? transformer, bool truncate)
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

        using var stream = File.OpenRead(_filePath);
        using var reader = await ParquetReader.CreateAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);

        var schema = reader.Schema;
        for (int g = 0; g < reader.RowGroupCount; g++)
        {
            using var groupReader = reader.OpenRowGroupReader(g);
            var columns = new DataColumn[schema.DataFields.Length];
            for (int c = 0; c < schema.DataFields.Length; c++)
            {
                columns[c] = await groupReader.ReadColumnAsync(schema.DataFields[c], cancellationToken).ConfigureAwait(false);
            }

            var rowCount = columns.Length > 0 ? columns[0].Data.Length : 0;
            for (int r = 0; r < rowCount; r++)
            {
                var row = new Dictionary<string, object?>();
                for (int c = 0; c < columns.Length; c++)
                {
                    row[schema.DataFields[c].Name] = columns[c].Data.GetValue(r);
                }

                yield return _transformer is not null ? _transformer(row) : row;
            }
        }
    }

    /// <inheritdoc/>
    public async Task<int> LengthAsync(CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
        {
            return 0;
        }

        using var stream = File.OpenRead(_filePath);
        using var reader = await ParquetReader.CreateAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);

        int count = 0;
        for (int g = 0; g < reader.RowGroupCount; g++)
        {
            using var groupReader = reader.OpenRowGroupReader(g);
            if (reader.Schema.DataFields.Length > 0)
            {
                var col = await groupReader.ReadColumnAsync(reader.Schema.DataFields[0], cancellationToken).ConfigureAwait(false);
                count += col.Data.Length;
            }
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

        var columnNames = _buffer[0].Keys.ToList();
        var dataFields = columnNames.Select(name => new DataField(name, typeof(string))).ToArray();
        var schema = new ParquetSchema(dataFields);

        using var stream = _truncate ? File.Create(_filePath) : File.OpenWrite(_filePath);
        using var writer = await ParquetWriter.CreateAsync(schema, stream, cancellationToken: cancellationToken).ConfigureAwait(false);

        using var groupWriter = writer.CreateRowGroup();
        for (int c = 0; c < dataFields.Length; c++)
        {
            var colName = columnNames[c];
            var values = _buffer.Select(r => r.TryGetValue(colName, out var v) ? v?.ToString() : null).ToArray();
            var dataColumn = new DataColumn(dataFields[c], values);
            await groupWriter.WriteColumnAsync(dataColumn, cancellationToken).ConfigureAwait(false);
        }

        _buffer.Clear();
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await CloseAsync().ConfigureAwait(false);
    }
}
