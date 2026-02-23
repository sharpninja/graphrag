// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Globalization;

namespace GraphRag.Vectors;

/// <summary>
/// Utility methods for working with ISO 8601 timestamps.
/// </summary>
public static class TimestampHelper
{
    /// <summary>
    /// Parses an ISO 8601 timestamp and returns a dictionary with its individual components.
    /// </summary>
    /// <param name="isoTimestamp">The ISO 8601 timestamp string to parse.</param>
    /// <param name="prefix">The prefix to use for the dictionary keys.</param>
    /// <returns>
    /// A dictionary containing year, month, day, hour, minute, and second fields,
    /// or an empty dictionary if the timestamp is <c>null</c> or cannot be parsed.
    /// </returns>
    public static Dictionary<string, object?> ExplodeTimestamp(string? isoTimestamp, string prefix = "create_date")
    {
        if (string.IsNullOrWhiteSpace(isoTimestamp))
        {
            return [];
        }

        if (!DateTimeOffset.TryParse(isoTimestamp, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
        {
            return [];
        }

        return new Dictionary<string, object?>
        {
            [$"{prefix}_year"] = dt.Year,
            [$"{prefix}_month"] = dt.Month,
            [$"{prefix}_day"] = dt.Day,
            [$"{prefix}_hour"] = dt.Hour,
            [$"{prefix}_minute"] = dt.Minute,
            [$"{prefix}_second"] = dt.Second,
        };
    }
}
