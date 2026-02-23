// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Globalization;

using GraphRag.Storage;

namespace GraphRag.Input;

/// <summary>
/// Reads CSV files from storage and produces <see cref="TextDocument"/> instances.
/// Uses a simple built-in CSV parser. For robust CSV parsing (quoted fields,
/// escaping, multi-line values), use the GraphRag.Input.CsvHelper strategy library.
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
        using var reader = new StringReader(content);

        // Read header line.
        var headerLine = await reader.ReadLineAsync(ct).ConfigureAwait(false);
        if (headerLine is null)
        {
            return rows;
        }

        var headers = ParseCsvLine(headerLine);

        // Read data lines.
        string? line;
        while ((line = await reader.ReadLineAsync(ct).ConfigureAwait(false)) is not null)
        {
            ct.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var fields = ParseCsvLine(line);
            var row = new Dictionary<string, object?>();
            for (var i = 0; i < headers.Length; i++)
            {
                row[headers[i]] = i < fields.Length ? fields[i] : null;
            }

            rows.Add(row);
        }

        return rows;
    }

    private static string[] ParseCsvLine(string line)
    {
        var fields = new List<string>();
        var inQuotes = false;
        var field = new System.Text.StringBuilder();

        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (inQuotes)
            {
                if (c == '"')
                {
                    if (i + 1 < line.Length && line[i + 1] == '"')
                    {
                        field.Append('"');
                        i++;
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    field.Append(c);
                }
            }
            else if (c == '"')
            {
                inQuotes = true;
            }
            else if (c == ',')
            {
                fields.Add(field.ToString());
                field.Clear();
            }
            else
            {
                field.Append(c);
            }
        }

        fields.Add(field.ToString());
        return fields.ToArray();
    }
}
