// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Text.Json;

using GraphRag.Storage;

namespace GraphRag.Input;

/// <summary>
/// Reads JSON Lines files from storage and produces <see cref="TextDocument"/> instances.
/// </summary>
public class JsonLinesFileReader : StructuredFileReader
{
    private const string DefaultFilePattern = @".*\.jsonl$";

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonLinesFileReader"/> class.
    /// </summary>
    /// <param name="storage">The storage to read files from.</param>
    /// <param name="encoding">The encoding to use when reading files.</param>
    /// <param name="filePattern">A regex pattern to match JSONL files.</param>
    /// <param name="idColumn">The column name to use as the document ID.</param>
    /// <param name="titleColumn">The column name to use as the document title.</param>
    /// <param name="textColumn">The column name to use as the document text.</param>
    public JsonLinesFileReader(
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

        string? line;
        while ((line = await reader.ReadLineAsync(ct).ConfigureAwait(false)) is not null)
        {
            ct.ThrowIfCancellationRequested();

            var trimmed = line.Trim();
            if (trimmed.Length == 0)
            {
                continue;
            }

            var element = JsonSerializer.Deserialize<JsonElement>(trimmed);
            if (element.ValueKind == JsonValueKind.Object)
            {
                rows.Add(JsonObjectToDict(element));
            }
        }

        return rows;
    }

    private static Dictionary<string, object?> JsonObjectToDict(JsonElement element)
    {
        var dict = new Dictionary<string, object?>();
        foreach (var property in element.EnumerateObject())
        {
            dict[property.Name] = ConvertJsonValue(property.Value);
        }

        return dict;
    }

    private static object? ConvertJsonValue(JsonElement value)
    {
        return value.ValueKind switch
        {
            JsonValueKind.String => value.GetString(),
            JsonValueKind.Number => value.TryGetInt64(out var l) ? l : value.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Null => null,
            JsonValueKind.Object => JsonObjectToDict(value),
            JsonValueKind.Array => value.EnumerateArray().Select(ConvertJsonValue).ToList(),
            _ => value.GetRawText(),
        };
    }
}
