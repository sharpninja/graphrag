// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Globalization;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using CsvHelper;
using CsvHelper.Configuration;

using GraphRag.Common.Discovery;
using GraphRag.Storage;

namespace GraphRag.Input.CsvHelper;

/// <summary>
/// Reads CSV files from storage using the CsvHelper library for robust parsing
/// of quoted fields, escaping, and multi-line values.
/// </summary>
[StrategyImplementation("csvhelper", typeof(IInputReader))]
public sealed class CsvHelperFileReader : IInputReader
{
    private const string DefaultFilePattern = @".*\.csv$";

    private readonly IStorage _storage;
    private readonly string _encoding;
    private readonly Regex _filePattern;
    private readonly string? _idColumn;
    private readonly string? _titleColumn;
    private readonly string? _textColumn;

    /// <summary>
    /// Initializes a new instance of the <see cref="CsvHelperFileReader"/> class.
    /// </summary>
    /// <param name="storage">The storage to read files from.</param>
    /// <param name="encoding">The encoding to use when reading files.</param>
    /// <param name="filePattern">A regex pattern to match CSV files.</param>
    /// <param name="idColumn">The column name to use as the document ID.</param>
    /// <param name="titleColumn">The column name to use as the document title.</param>
    /// <param name="textColumn">The column name to use as the document text.</param>
    public CsvHelperFileReader(
        IStorage storage,
        string? encoding = null,
        string? filePattern = null,
        string? idColumn = null,
        string? titleColumn = null,
        string? textColumn = null)
    {
        _storage = storage;
        _encoding = encoding ?? "utf-8";
        _filePattern = new Regex(filePattern ?? DefaultFilePattern, RegexOptions.Compiled);
        _idColumn = idColumn;
        _titleColumn = titleColumn;
        _textColumn = textColumn;
    }

    /// <inheritdoc />
    public async Task<List<TextDocument>> ReadFilesAsync(CancellationToken ct = default)
    {
        var documents = new List<TextDocument>();
        await foreach (var doc in ReadAsync(ct).ConfigureAwait(false))
        {
            documents.Add(doc);
        }

        return documents;
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<TextDocument> ReadAsync(
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var files = _storage.Find(_filePattern);
        foreach (var file in files)
        {
            ct.ThrowIfCancellationRequested();

            var content = await _storage.GetAsync(file, encoding: _encoding, cancellationToken: ct)
                .ConfigureAwait(false) as string;
            if (content is null)
            {
                continue;
            }

            var creationDate = await _storage.GetCreationDateAsync(file, ct).ConfigureAwait(false);

            using var reader = new StringReader(content);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
            });

            await csv.ReadAsync().ConfigureAwait(false);
            csv.ReadHeader();
            var headers = csv.HeaderRecord ?? [];

            var rowIndex = 0;
            while (await csv.ReadAsync().ConfigureAwait(false))
            {
                ct.ThrowIfCancellationRequested();

                var row = new Dictionary<string, object?>();
                foreach (var header in headers)
                {
                    row[header] = csv.GetField(header);
                }

                var text = _textColumn is not null && row.TryGetValue(_textColumn, out var tv)
                    ? tv?.ToString() ?? string.Empty
                    : string.Empty;

                string id;
                if (_idColumn is not null && row.TryGetValue(_idColumn, out var idVal) && idVal is not null)
                {
                    id = idVal.ToString()!;
                }
                else
                {
                    id = Convert.ToHexString(
                        SHA512.HashData(Encoding.UTF8.GetBytes($"{file}:{rowIndex}:{text}")))
                        .ToLowerInvariant();
                }

                string title;
                if (_titleColumn is not null && row.TryGetValue(_titleColumn, out var titleVal) && titleVal is not null)
                {
                    title = titleVal.ToString()!;
                }
                else
                {
                    title = $"{file}:{rowIndex}";
                }

                yield return new TextDocument(
                    Id: id,
                    Text: text,
                    Title: title,
                    CreationDate: creationDate,
                    RawData: row);

                rowIndex++;
            }
        }
    }
}
