// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using GraphRag.Storage;

namespace GraphRag.Input;

/// <summary>
/// Reads plain text files from storage and produces <see cref="TextDocument"/> instances.
/// </summary>
public class TextFileReader : IInputReader
{
    private const string DefaultFilePattern = @".*\.txt$";

    private readonly IStorage _storage;
    private readonly string _encoding;
    private readonly Regex _filePattern;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextFileReader"/> class.
    /// </summary>
    /// <param name="storage">The storage to read files from.</param>
    /// <param name="encoding">The encoding to use when reading files.</param>
    /// <param name="filePattern">A regex pattern to match text files.</param>
    public TextFileReader(IStorage storage, string? encoding = null, string? filePattern = null)
    {
        _storage = storage;
        _encoding = encoding ?? "utf-8";
        _filePattern = new Regex(filePattern ?? DefaultFilePattern, RegexOptions.Compiled);
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

            var text = await _storage.GetAsync(file, encoding: _encoding, cancellationToken: ct)
                .ConfigureAwait(false) as string;
            if (text is null)
            {
                continue;
            }

            var creationDate = await _storage.GetCreationDateAsync(file, ct).ConfigureAwait(false);
            var hash = Convert.ToHexString(
                SHA512.HashData(Encoding.UTF8.GetBytes(text))).ToLowerInvariant();

            yield return new TextDocument(
                Id: hash,
                Text: text,
                Title: file,
                CreationDate: creationDate);
        }
    }
}
