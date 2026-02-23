// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Text.Json;

using GraphRag.Storage;

namespace GraphRag.Input;

/// <summary>
/// Reads JSON files from storage and produces <see cref="TextDocument"/> instances.
/// </summary>
public class JsonFileReader : StructuredFileReader
{
    private const string DefaultFilePattern = @".*\.json$";

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonFileReader"/> class.
    /// </summary>
    /// <param name="storage">The storage to read files from.</param>
    /// <param name="encoding">The encoding to use when reading files.</param>
    /// <param name="filePattern">A regex pattern to match JSON files.</param>
    /// <param name="idColumn">The column name to use as the document ID.</param>
    /// <param name="titleColumn">The column name to use as the document title.</param>
    /// <param name="textColumn">The column name to use as the document text.</param>
    public JsonFileReader(
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

        var document = JsonSerializer.Deserialize<JsonElement>(content);
        return ParseJsonElement(document);
    }

    private static List<Dictionary<string, object?>> ParseJsonElement(JsonElement element)
    {
        var rows = new List<Dictionary<string, object?>>();

        if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in element.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.Object)
                {
                    rows.Add(JsonObjectToDict(item));
                }
            }
        }
        else if (element.ValueKind == JsonValueKind.Object)
        {
            rows.Add(JsonObjectToDict(element));
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
