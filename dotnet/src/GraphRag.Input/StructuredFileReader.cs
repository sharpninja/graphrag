// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using GraphRag.Storage;

namespace GraphRag.Input;

/// <summary>
/// Abstract base class for reading structured input files (CSV, JSON, JSONL) from storage.
/// </summary>
public abstract class StructuredFileReader : IInputReader
{
    private readonly IStorage _storage;
    private readonly string _encoding;
    private readonly Regex _filePattern;
    private readonly string? _idColumn;
    private readonly string? _titleColumn;
    private readonly string? _textColumn;

    /// <summary>
    /// Initializes a new instance of the <see cref="StructuredFileReader"/> class.
    /// </summary>
    /// <param name="storage">The storage to read files from.</param>
    /// <param name="encoding">The encoding to use when reading files.</param>
    /// <param name="filePattern">A regex pattern to match input files.</param>
    /// <param name="defaultPattern">The default regex pattern for the file type.</param>
    /// <param name="idColumn">The column name to use as the document ID.</param>
    /// <param name="titleColumn">The column name to use as the document title.</param>
    /// <param name="textColumn">The column name to use as the document text.</param>
    protected StructuredFileReader(
        IStorage storage,
        string? encoding,
        string? filePattern,
        string defaultPattern,
        string? idColumn,
        string? titleColumn,
        string? textColumn)
    {
        _storage = storage;
        _encoding = encoding ?? "utf-8";
        _filePattern = new Regex(filePattern ?? defaultPattern, RegexOptions.Compiled);
        _idColumn = idColumn;
        _titleColumn = titleColumn;
        _textColumn = textColumn;
    }

    /// <summary>
    /// Gets the storage instance.
    /// </summary>
    protected IStorage Storage => _storage;

    /// <summary>
    /// Gets the encoding to use when reading files.
    /// </summary>
    protected string Encoding => _encoding;

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

            var rows = await ParseFileAsync(file, ct).ConfigureAwait(false);
            if (rows is null || rows.Count == 0)
            {
                continue;
            }

            var creationDate = await _storage.GetCreationDateAsync(file, ct).ConfigureAwait(false);
            var documents = ProcessDataColumns(rows, file);

            foreach (var doc in documents)
            {
                yield return doc with { CreationDate = creationDate };
            }
        }
    }

    /// <summary>
    /// Parses a single file into a list of row dictionaries.
    /// </summary>
    /// <param name="path">The file path within storage.</param>
    /// <param name="ct">A token to cancel the operation.</param>
    /// <returns>A list of dictionaries representing the rows in the file.</returns>
    protected abstract Task<List<Dictionary<string, object?>>?> ParseFileAsync(
        string path, CancellationToken ct);

    /// <summary>
    /// Processes rows into <see cref="TextDocument"/> instances using the configured column mappings.
    /// </summary>
    /// <param name="rows">The rows to process.</param>
    /// <param name="path">The source file path, used for generating default titles.</param>
    /// <returns>A list of text documents.</returns>
    protected List<TextDocument> ProcessDataColumns(
        List<Dictionary<string, object?>> rows, string path)
    {
        var documents = new List<TextDocument>();

        for (var i = 0; i < rows.Count; i++)
        {
            var row = rows[i];

            // Resolve text
            var text = _textColumn is not null && row.TryGetValue(_textColumn, out var tv)
                ? tv?.ToString() ?? string.Empty
                : string.Empty;

            // Resolve ID
            string id;
            if (_idColumn is not null && row.TryGetValue(_idColumn, out var idVal) && idVal is not null)
            {
                id = idVal.ToString()!;
            }
            else
            {
                var columns = row.Keys.ToArray();
                id = PropertyHelper.GenSha512Hash(row, columns);
            }

            // Resolve title
            string title;
            if (_titleColumn is not null && row.TryGetValue(_titleColumn, out var titleVal) && titleVal is not null)
            {
                title = titleVal.ToString()!;
            }
            else
            {
                title = $"{path}:{i}";
            }

            documents.Add(new TextDocument(
                Id: id,
                Text: text,
                Title: title,
                CreationDate: string.Empty,
                RawData: row));
        }

        return documents;
    }
}
