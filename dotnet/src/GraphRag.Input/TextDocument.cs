// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Input;

/// <summary>
/// Represents a text document read from an input source.
/// </summary>
/// <param name="Id">The unique identifier of the document.</param>
/// <param name="Text">The text content of the document.</param>
/// <param name="Title">The title of the document.</param>
/// <param name="CreationDate">The creation date of the document in ISO-8601 format.</param>
/// <param name="RawData">Optional dictionary containing the original source data.</param>
public sealed record TextDocument(
    string Id,
    string Text,
    string Title,
    string CreationDate,
    Dictionary<string, object?>? RawData = null)
{
    /// <summary>
    /// Retrieves a field value by name, checking standard properties first and then <see cref="RawData"/>.
    /// </summary>
    /// <param name="field">The field name to retrieve.</param>
    /// <param name="defaultValue">The default value to return if the field is not found.</param>
    /// <returns>The field value, or <paramref name="defaultValue"/> if not found.</returns>
    public object? Get(string field, object? defaultValue = null)
    {
        return field.ToLowerInvariant() switch
        {
            "id" => Id,
            "text" => Text,
            "title" => Title,
            "creation_date" or "creationdate" => CreationDate,
            _ => RawData is not null && RawData.TryGetValue(field, out var value) ? value : defaultValue,
        };
    }

    /// <summary>
    /// Extracts multiple field values into a dictionary, skipping fields with null values.
    /// </summary>
    /// <param name="fields">The list of field names to collect.</param>
    /// <returns>A dictionary of field names to non-null values.</returns>
    public Dictionary<string, object?> Collect(IReadOnlyList<string> fields)
    {
        var result = new Dictionary<string, object?>();
        foreach (var field in fields)
        {
            var value = Get(field);
            if (value is not null)
            {
                result[field] = value;
            }
        }

        return result;
    }
}
