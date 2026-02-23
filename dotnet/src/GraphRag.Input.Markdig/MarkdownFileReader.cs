// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using GraphRag.Common.Discovery;
using GraphRag.Storage;

using Markdig;

namespace GraphRag.Input.Markdig;

/// <summary>
/// Reads Markdown files from storage and strips formatting using the Markdig library,
/// producing plain-text <see cref="TextDocument"/> instances.
/// </summary>
[StrategyImplementation("markdown", typeof(IInputReader))]
public sealed class MarkdownFileReader : IInputReader
{
    private const string DefaultFilePattern = @".*\.md$";

    private readonly IStorage _storage;
    private readonly string _encoding;
    private readonly Regex _filePattern;

    /// <summary>
    /// Initializes a new instance of the <see cref="MarkdownFileReader"/> class.
    /// </summary>
    /// <param name="storage">The storage to read files from.</param>
    /// <param name="encoding">The encoding to use when reading files.</param>
    /// <param name="filePattern">A regex pattern to match Markdown files.</param>
    public MarkdownFileReader(
        IStorage storage,
        string? encoding = null,
        string? filePattern = null)
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

            var markdown = await _storage.GetAsync(file, encoding: _encoding, cancellationToken: ct)
                .ConfigureAwait(false) as string;
            if (markdown is null)
            {
                continue;
            }

            var plainText = Markdown.ToPlainText(markdown);
            var creationDate = await _storage.GetCreationDateAsync(file, ct).ConfigureAwait(false);
            var hash = Convert.ToHexString(
                SHA512.HashData(Encoding.UTF8.GetBytes(plainText))).ToLowerInvariant();

            yield return new TextDocument(
                Id: hash,
                Text: plainText,
                Title: file,
                CreationDate: creationDate);
        }
    }
}
