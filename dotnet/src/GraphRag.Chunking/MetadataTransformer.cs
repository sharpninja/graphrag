// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Chunking;

/// <summary>
/// Provides helper methods for creating text transformation functions that add metadata to chunks.
/// </summary>
public static class MetadataTransformer
{
    /// <summary>
    /// Create a transformation function that prepends or appends metadata key-value pairs to chunk text.
    /// </summary>
    /// <param name="metadata">The metadata key-value pairs to include.</param>
    /// <param name="delimiter">The delimiter between key and value. Defaults to ": ".</param>
    /// <param name="lineDelimiter">The delimiter between metadata lines. Defaults to newline.</param>
    /// <param name="append">If <c>true</c>, metadata is appended; otherwise it is prepended.</param>
    /// <returns>A function that transforms text by adding metadata.</returns>
    public static Func<string, string> AddMetadata(
        Dictionary<string, object?> metadata,
        string delimiter = ": ",
        string lineDelimiter = "\n",
        bool append = false)
    {
        var metadataLines = new List<string>();
        foreach (var kvp in metadata)
        {
            if (kvp.Value is not null)
            {
                metadataLines.Add($"{kvp.Key}{delimiter}{kvp.Value}");
            }
        }

        var metadataBlock = string.Join(lineDelimiter, metadataLines);

        return text =>
        {
            if (metadataLines.Count == 0)
            {
                return text;
            }

            return append
                ? $"{text}{lineDelimiter}{metadataBlock}"
                : $"{metadataBlock}{lineDelimiter}{text}";
        };
    }
}
