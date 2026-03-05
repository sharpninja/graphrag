// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Globalization;

namespace GraphRag.DataModel;

/// <summary>
/// Row-level type coercion helpers for streaming table reads.
/// </summary>
/// <remarks>
/// Each transformer converts a raw <see cref="Dictionary{TKey,TValue}"/>
/// row (as produced by CSV/Parquet readers, where values may be strings)
/// into a dictionary with properly typed fields.
/// </remarks>
public static class RowTransformers
{
    /// <summary>
    /// Coerces a value to an integer, returning <paramref name="fill"/> when missing or empty.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="fill">The default value when conversion fails.</param>
    /// <returns>The converted integer value.</returns>
    public static int SafeInt(object? value, int fill = -1)
    {
        if (value is null)
        {
            return fill;
        }

        if (value is int i)
        {
            return i;
        }

        if (value is long l)
        {
            return (int)l;
        }

        var s = value.ToString();
        if (string.IsNullOrEmpty(s))
        {
            return fill;
        }

        return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : fill;
    }

    /// <summary>
    /// Coerces a value to a double, returning <paramref name="fill"/> when missing or empty.
    /// </summary>
    /// <param name="value">The value to convert.</param>
    /// <param name="fill">The default value when conversion fails.</param>
    /// <returns>The converted double value.</returns>
    public static double SafeFloat(object? value, double fill = 0.0)
    {
        if (value is null)
        {
            return fill;
        }

        if (value is double d)
        {
            return double.IsNaN(d) ? fill : d;
        }

        if (value is float f)
        {
            return float.IsNaN(f) ? fill : f;
        }

        var s = value.ToString();
        if (string.IsNullOrEmpty(s))
        {
            return fill;
        }

        return double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out var parsed)
            ? (double.IsNaN(parsed) ? fill : parsed)
            : fill;
    }

    /// <summary>
    /// Parses a value into a list, handling CSV-encoded strings and array types.
    /// </summary>
    /// <param name="value">The value to parse.</param>
    /// <returns>A list of string values.</returns>
    public static List<string> CoerceList(object? value)
    {
        if (value is null)
        {
            return [];
        }

        if (value is List<string> list)
        {
            return list;
        }

        if (value is IReadOnlyList<string> readOnlyList)
        {
            return [.. readOnlyList];
        }

        if (value is IEnumerable<string> enumerable)
        {
            return [.. enumerable];
        }

        if (value is string s)
        {
            return ParseListString(s);
        }

        return [];
    }

    /// <summary>
    /// Coerces types for an entity row.
    /// </summary>
    /// <param name="row">The raw row dictionary.</param>
    /// <returns>The same dictionary with coerced types.</returns>
    public static Dictionary<string, object?> TransformEntityRow(Dictionary<string, object?> row)
    {
        if (row.TryGetValue("human_readable_id", out var hrid))
        {
            row["human_readable_id"] = SafeInt(hrid);
        }

        if (row.TryGetValue("text_unit_ids", out var tuIds))
        {
            row["text_unit_ids"] = CoerceList(tuIds);
        }

        if (row.TryGetValue("frequency", out var freq))
        {
            row["frequency"] = SafeInt(freq, 0);
        }

        if (row.TryGetValue("degree", out var deg))
        {
            row["degree"] = SafeInt(deg, 0);
        }

        return row;
    }

    /// <summary>
    /// Adds a title_description column for embedding generation.
    /// </summary>
    /// <param name="row">The raw row dictionary.</param>
    /// <returns>The same dictionary with the added column.</returns>
    public static Dictionary<string, object?> TransformEntityRowForEmbedding(Dictionary<string, object?> row)
    {
        var title = row.TryGetValue("title", out var t) ? t?.ToString() ?? string.Empty : string.Empty;
        var description = row.TryGetValue("description", out var d) ? d?.ToString() ?? string.Empty : string.Empty;
        row["title_description"] = $"{title}:{description}";
        return row;
    }

    /// <summary>
    /// Coerces types for a relationship row.
    /// </summary>
    /// <param name="row">The raw row dictionary.</param>
    /// <returns>The same dictionary with coerced types.</returns>
    public static Dictionary<string, object?> TransformRelationshipRow(Dictionary<string, object?> row)
    {
        if (row.TryGetValue("human_readable_id", out var hrid))
        {
            row["human_readable_id"] = SafeInt(hrid);
        }

        if (row.TryGetValue("weight", out var weight))
        {
            row["weight"] = SafeFloat(weight);
        }

        if (row.TryGetValue("combined_degree", out var cd))
        {
            row["combined_degree"] = SafeInt(cd, 0);
        }

        if (row.TryGetValue("text_unit_ids", out var tuIds))
        {
            row["text_unit_ids"] = CoerceList(tuIds);
        }

        return row;
    }

    /// <summary>
    /// Coerces types for a community row.
    /// </summary>
    /// <param name="row">The raw row dictionary.</param>
    /// <returns>The same dictionary with coerced types.</returns>
    public static Dictionary<string, object?> TransformCommunityRow(Dictionary<string, object?> row)
    {
        if (row.TryGetValue("human_readable_id", out var hrid))
        {
            row["human_readable_id"] = SafeInt(hrid);
        }

        row["community"] = SafeInt(row.GetValueOrDefault("community"));
        row["level"] = SafeInt(row.GetValueOrDefault("level"));
        row["children"] = CoerceList(row.GetValueOrDefault("children"));

        if (row.TryGetValue("entity_ids", out var entityIds))
        {
            row["entity_ids"] = CoerceList(entityIds);
        }

        if (row.TryGetValue("relationship_ids", out var relIds))
        {
            row["relationship_ids"] = CoerceList(relIds);
        }

        if (row.TryGetValue("text_unit_ids", out var tuIds))
        {
            row["text_unit_ids"] = CoerceList(tuIds);
        }

        row["period"] = (row.GetValueOrDefault("period") ?? string.Empty).ToString() ?? string.Empty;
        row["size"] = SafeInt(row.GetValueOrDefault("size"), 0);

        return row;
    }

    /// <summary>
    /// Coerces types for a community report row.
    /// </summary>
    /// <param name="row">The raw row dictionary.</param>
    /// <returns>The same dictionary with coerced types.</returns>
    public static Dictionary<string, object?> TransformCommunityReportRow(Dictionary<string, object?> row)
    {
        if (row.TryGetValue("human_readable_id", out var hrid))
        {
            row["human_readable_id"] = SafeInt(hrid);
        }

        row["community"] = SafeInt(row.GetValueOrDefault("community"));
        row["level"] = SafeInt(row.GetValueOrDefault("level"));
        row["children"] = CoerceList(row.GetValueOrDefault("children"));
        row["rank"] = SafeFloat(row.GetValueOrDefault("rank"));
        row["findings"] = CoerceList(row.GetValueOrDefault("findings"));
        row["size"] = SafeInt(row.GetValueOrDefault("size"), 0);

        return row;
    }

    /// <summary>
    /// Coerces types for a covariate row.
    /// </summary>
    /// <param name="row">The raw row dictionary.</param>
    /// <returns>The same dictionary with coerced types.</returns>
    public static Dictionary<string, object?> TransformCovariateRow(Dictionary<string, object?> row)
    {
        if (row.TryGetValue("human_readable_id", out var hrid))
        {
            row["human_readable_id"] = SafeInt(hrid);
        }

        return row;
    }

    /// <summary>
    /// Coerces types for a text unit row.
    /// </summary>
    /// <param name="row">The raw row dictionary.</param>
    /// <returns>The same dictionary with coerced types.</returns>
    public static Dictionary<string, object?> TransformTextUnitRow(Dictionary<string, object?> row)
    {
        if (row.TryGetValue("human_readable_id", out var hrid))
        {
            row["human_readable_id"] = SafeInt(hrid);
        }

        row["n_tokens"] = SafeInt(row.GetValueOrDefault("n_tokens"), 0);

        if (row.TryGetValue("entity_ids", out var entityIds))
        {
            row["entity_ids"] = CoerceList(entityIds);
        }

        if (row.TryGetValue("relationship_ids", out var relIds))
        {
            row["relationship_ids"] = CoerceList(relIds);
        }

        if (row.TryGetValue("covariate_ids", out var covIds))
        {
            row["covariate_ids"] = CoerceList(covIds);
        }

        return row;
    }

    /// <summary>
    /// Coerces types for a document row.
    /// </summary>
    /// <param name="row">The raw row dictionary.</param>
    /// <returns>The same dictionary with coerced types.</returns>
    public static Dictionary<string, object?> TransformDocumentRow(Dictionary<string, object?> row)
    {
        if (row.TryGetValue("human_readable_id", out var hrid))
        {
            row["human_readable_id"] = SafeInt(hrid);
        }

        if (row.TryGetValue("text_unit_ids", out var tuIds))
        {
            row["text_unit_ids"] = CoerceList(tuIds);
        }

        return row;
    }

    /// <summary>
    /// Parses a CSV-encoded list string (e.g. <c>"[a, b, c]"</c>) into a list.
    /// </summary>
    /// <param name="value">The string value to parse.</param>
    /// <returns>A list of trimmed, non-empty string values.</returns>
    internal static List<string> ParseListString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return [];
        }

        // Strip surrounding brackets if present.
        var trimmed = value.Trim();
        if (trimmed.StartsWith('[') && trimmed.EndsWith(']'))
        {
            trimmed = trimmed[1..^1];
        }

        if (string.IsNullOrWhiteSpace(trimmed))
        {
            return [];
        }

        var parts = trimmed.Split(',');
        var result = new List<string>(parts.Length);
        foreach (var part in parts)
        {
            var item = part.Trim().Trim('\'', '"');
            if (!string.IsNullOrEmpty(item))
            {
                result.Add(item);
            }
        }

        return result;
    }
}
