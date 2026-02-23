// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Utils;

/// <summary>
/// Provides helper methods for text manipulation.
/// </summary>
public static class TextHelper
{
    /// <summary>
    /// Truncates the specified text to the given maximum length, appending an ellipsis if truncated.
    /// </summary>
    /// <param name="text">The text to truncate.</param>
    /// <param name="maxLength">The maximum allowed length.</param>
    /// <returns>The truncated text.</returns>
    public static string Truncate(string text, int maxLength)
    {
        ArgumentNullException.ThrowIfNull(text);
        if (maxLength < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(maxLength), "Maximum length must be non-negative.");
        }

        return text.Length <= maxLength ? text : string.Concat(text.AsSpan(0, maxLength), "...");
    }

    /// <summary>
    /// Loads a search prompt from the specified configuration path, or returns <c>null</c> if no path is provided.
    /// </summary>
    /// <param name="promptConfig">The file path to load the prompt from, or <c>null</c>.</param>
    /// <returns>The prompt text, or <c>null</c> if <paramref name="promptConfig"/> is <c>null</c> or empty.</returns>
    public static string? LoadSearchPrompt(string? promptConfig)
    {
        if (string.IsNullOrWhiteSpace(promptConfig))
        {
            return null;
        }

        return File.ReadAllText(promptConfig);
    }
}
