// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Globalization;
using System.IO;

using CsvHelper;
using CsvHelper.Configuration;

using GraphRag.Storage;

namespace GraphRag.Input;

/// <summary>
/// Reads CSV files from storage and produces <see cref="TextDocument"/> instances.
/// </summary>
public class CsvFileReader : StructuredFileReader
{
    private const string DefaultFilePattern = @".*\.csv$";

    /// <summary>
    /// Initializes a new instance of the <see cref="CsvFileReader"/> class.
    /// </summary>
    /// <param name="storage">The storage to read files from.</param>
    /// <param name="encoding">The encoding to use when reading files.</param>
    /// <param name="filePattern">A regex pattern to match CSV files.</param>
    /// <param name="idColumn">The column name to use as the document ID.</param>
    /// <param name="titleColumn">The column name to use as the document title.</param>
    /// <param name="textColumn">The column name to use as the document text.</param>
    public CsvFileReader(
        IStorage storage,
        string? encoding = null,
        string? filePattern = null,
        string? idColumn = null,
        string? titleColumn = null,
        string? textColumn = null)
        : base(storage, encoding, filePattern, DefaultFilePattern, idColumn, titleColumn, textColumn)
    {
    }

    /// <inheritdoc />
    protected override async Task<List<Dictionary<string, object?>>?> ParseFileAsync(
        string path, CancellationToken ct)
    {
        var content = await Storage.GetAsync(path, encoding: Encoding, cancellationToken: ct)
            .ConfigureAwait(false) as string;
        if (content is null)
        {
            return null;
        }

        var rows = new List<Dictionary<string, object?>>();
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
        };

        using var reader = new StringReader(content);
        using var csv = new CsvReader(reader, config);

        await csv.ReadAsync().ConfigureAwait(false);
        csv.ReadHeader();
        var headers = csv.HeaderRecord ?? [];

        while (await csv.ReadAsync().ConfigureAwait(false))
        {
            ct.ThrowIfCancellationRequested();

            var row = new Dictionary<string, object?>();
            foreach (var header in headers)
            {
                row[header] = csv.GetField(header);
            }

            rows.Add(row);
        }

        return rows;
    }
}
