// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Security.Cryptography;
using System.Text;

namespace GraphRag.Input;

/// <summary>
/// Provides utility methods for property access and hashing.
/// </summary>
public static class PropertyHelper
{
    /// <summary>
    /// Retrieves a property value from a dictionary using dot-notation path traversal.
    /// </summary>
    /// <param name="data">The source dictionary.</param>
    /// <param name="path">The dot-separated path to the property.</param>
    /// <returns>The property value, or <c>null</c> if not found.</returns>
    public static object? GetProperty(Dictionary<string, object?> data, string path)
    {
        var segments = path.Split('.');
        object? current = data;

        foreach (var segment in segments)
        {
            if (current is Dictionary<string, object?> dict)
            {
                if (!dict.TryGetValue(segment, out current))
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        return current;
    }

    /// <summary>
    /// Generates a SHA-512 hash from the concatenated values of the specified columns.
    /// </summary>
    /// <param name="item">The source dictionary.</param>
    /// <param name="columns">The column names whose values are concatenated for hashing.</param>
    /// <returns>A lowercase hexadecimal SHA-512 hash string.</returns>
    public static string GenSha512Hash(Dictionary<string, object?> item, params string[] columns)
    {
        var sb = new StringBuilder();
        foreach (var col in columns)
        {
            if (item.TryGetValue(col, out var value) && value is not null)
            {
                sb.Append(value.ToString());
            }
        }

        var bytes = SHA512.HashData(Encoding.UTF8.GetBytes(sb.ToString()));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
