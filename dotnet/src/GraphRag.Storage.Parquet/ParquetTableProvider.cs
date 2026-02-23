// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Common.Discovery;
using GraphRag.Storage.Tables;

using Parquet;
using Parquet.Data;
using Parquet.Schema;

namespace GraphRag.Storage.Parquet;

/// <summary>
/// Parquet file-based implementation of <see cref="ITableProvider"/>.
/// </summary>
[StrategyImplementation("parquet", typeof(ITableProvider))]
public sealed class ParquetTableProvider : ITableProvider
{
    private readonly string _baseDir;

    /// <summary>
    /// Initializes a new instance of the <see cref="ParquetTableProvider"/> class.
    /// </summary>
    /// <param name="baseDir">The base directory containing Parquet files.</param>
    public ParquetTableProvider(string baseDir)
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

        using var stream = File.OpenRead(filePath);
        using var reader = await ParquetReader.CreateAsync(stream, cancellationToken: cancellationToken).ConfigureAwait(false);

        var rows = new List<Dictionary<string, object?>>();
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

                rows.Add(row);
            }
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

        var columnNames = rows[0].Keys.ToList();
        var dataFields = columnNames.Select(name => new DataField(name, typeof(string))).ToArray();
        var schema = new ParquetSchema(dataFields);

        var filePath = GetFilePath(tableName);
        using var stream = File.Create(filePath);
        using var writer = await ParquetWriter.CreateAsync(schema, stream, cancellationToken: cancellationToken).ConfigureAwait(false);

        using var groupWriter = writer.CreateRowGroup();
        for (int c = 0; c < dataFields.Length; c++)
        {
            var columnName = columnNames[c];
            var values = rows.Select(r => r.TryGetValue(columnName, out var v) ? v?.ToString() : null).ToArray();
            var dataColumn = new DataColumn(dataFields[c], values);
            await groupWriter.WriteColumnAsync(dataColumn, cancellationToken).ConfigureAwait(false);
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

        return Directory.EnumerateFiles(_baseDir, "*.parquet")
            .Select(f => Path.GetFileNameWithoutExtension(f))
            .ToList();
    }

    /// <inheritdoc/>
    public ITable Open(string tableName, Func<Dictionary<string, object?>, object>? transformer = null, bool truncate = true)
    {
        return new ParquetTable(GetFilePath(tableName), tableName, transformer, truncate);
    }

    private string GetFilePath(string tableName)
    {
        return Path.Combine(_baseDir, $"{tableName}.parquet");
    }
}
