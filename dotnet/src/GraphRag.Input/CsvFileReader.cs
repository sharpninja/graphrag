// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Storage;

namespace GraphRag.Input;

/// <summary>
/// Reads CSV files from storage and produces <see cref="TextDocument"/> instances.
/// Handles quoted fields, escaped double-quotes, and multi-line field values.
/// For additional CSV parsing features, use the GraphRag.Input.CsvHelper strategy library.
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

        var allRows = ParseCsvContent(content);
        if (allRows.Count == 0)
        {
            return [];
        }

        var headers = allRows[0];
        if (Array.TrueForAll(headers, string.IsNullOrWhiteSpace))
        {
            return [];
        }

        var rows = new List<Dictionary<string, object?>>();

        for (var i = 1; i < allRows.Count; i++)
        {
            ct.ThrowIfCancellationRequested();

            var fields = allRows[i];

            // Skip blank rows (a single empty field means an empty line).
            if (fields.Length == 1 && string.IsNullOrWhiteSpace(fields[0]))
            {
                continue;
            }

            var row = new Dictionary<string, object?>();
            for (var j = 0; j < headers.Length; j++)
            {
                row[headers[j]] = j < fields.Length ? fields[j] : null;
            }

            rows.Add(row);
        }

        return rows;
    }

    /// <summary>
    /// Parses CSV content into rows, correctly handling quoted fields that may contain
    /// embedded newlines, commas, and escaped double-quotes (<c>""</c>).
    /// </summary>
    private static List<string[]> ParseCsvContent(string content)
    {
        var rows = new List<string[]>();
        var fields = new List<string>();
        var field = new System.Text.StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < content.Length; i++)
        {
            var c = content[i];

            if (inQuotes)
            {
                if (c == '"')
                {
                    // Escaped quote: "" inside a quoted field → single "
                    if (i + 1 < content.Length && content[i + 1] == '"')
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
                    // Append everything verbatim inside quotes, including \r and \n.
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
            else if (c == '\r' || c == '\n')
            {
                // End of row — consume \r\n as a single line ending.
                fields.Add(field.ToString());
                field.Clear();
                rows.Add([.. fields]);
                fields.Clear();

                if (c == '\r' && i + 1 < content.Length && content[i + 1] == '\n')
                {
                    i++;
                }
            }
            else
            {
                field.Append(c);
            }
        }

        // Flush trailing content that is not terminated by a newline.
        // Only add a row if there is actual non-empty content remaining.
        if (field.Length > 0 || fields.Count > 0)
        {
            fields.Add(field.ToString());
            if (!fields.TrueForAll(string.IsNullOrEmpty))
            {
                rows.Add([.. fields]);
            }
        }

        return rows;
    }
}
