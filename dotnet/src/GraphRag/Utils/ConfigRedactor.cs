// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Utils;

/// <summary>
/// Provides methods to redact sensitive values from configuration dictionaries.
/// </summary>
public static class ConfigRedactor
{
    private const string RedactedValue = "***REDACTED***";

    private static readonly HashSet<string> SensitiveKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "api_key",
        "apikey",
        "api_secret",
        "secret",
        "password",
        "connection_string",
        "connectionstring",
        "token",
        "access_token",
        "refresh_token",
    };

    /// <summary>
    /// Returns a new dictionary with sensitive values masked.
    /// </summary>
    /// <param name="config">The configuration dictionary to redact.</param>
    /// <returns>A new dictionary with sensitive values replaced by a redacted placeholder.</returns>
    public static Dictionary<string, object?> Redact(Dictionary<string, object?> config)
    {
        ArgumentNullException.ThrowIfNull(config);

        var redacted = new Dictionary<string, object?>(config.Count, StringComparer.OrdinalIgnoreCase);

        foreach (var kvp in config)
        {
            redacted[kvp.Key] = SensitiveKeys.Contains(kvp.Key) ? RedactedValue : kvp.Value;
        }

        return redacted;
    }
}
