// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

using GraphRag.Common.Discovery;
using GraphRag.Storage;

namespace GraphRag.Input.OpenXml;

/// <summary>
/// Reads .docx files from storage using the Open XML SDK and extracts their text content
/// as <see cref="TextDocument"/> instances.
/// </summary>
[StrategyImplementation("openxml", typeof(IInputReader))]
public sealed class OpenXmlFileReader : IInputReader
{
    private const string DefaultFilePattern = @".*\.docx$";

    private readonly IStorage _storage;
    private readonly Regex _filePattern;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenXmlFileReader"/> class.
    /// </summary>
    /// <param name="storage">The storage to read files from.</param>
    /// <param name="filePattern">A regex pattern to match .docx files.</param>
    public OpenXmlFileReader(
        IStorage storage,
        string? filePattern = null)
    {
        _storage = storage;
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

            var data = await _storage.GetAsync(file, asBytes: true, cancellationToken: ct)
                .ConfigureAwait(false);
            if (data is not byte[] bytes)
            {
                continue;
            }

            var text = ExtractText(bytes);
            if (string.IsNullOrWhiteSpace(text))
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

    private static string ExtractText(byte[] docxBytes)
    {
        using var stream = new MemoryStream(docxBytes);
        using var wordDoc = WordprocessingDocument.Open(stream, false);
        var body = wordDoc.MainDocumentPart?.Document.Body;
        if (body is null)
        {
            return string.Empty;
        }

        var sb = new StringBuilder();
        foreach (var paragraph in body.Elements<Paragraph>())
        {
            sb.AppendLine(paragraph.InnerText);
        }

        return sb.ToString().TrimEnd();
    }
}
