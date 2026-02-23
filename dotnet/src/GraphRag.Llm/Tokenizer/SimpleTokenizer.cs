// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm.Tokenizer;

/// <summary>
/// A simple whitespace-based tokenizer for testing and default use.
/// Splits text on spaces and assigns sequential token identifiers.
/// </summary>
public sealed class SimpleTokenizer : ITokenizer
{
    private static readonly char[] Separator = [' '];

    /// <inheritdoc />
    public IReadOnlyList<int> Encode(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return Array.Empty<int>();
        }

        var words = text.Split(Separator, StringSplitOptions.RemoveEmptyEntries);
        var tokens = new int[words.Length];
        for (var i = 0; i < words.Length; i++)
        {
            tokens[i] = i;
        }

        return tokens;
    }

    /// <inheritdoc />
    public string Decode(IReadOnlyList<int> tokens)
    {
        // Since this is a simple tokenizer that doesn't maintain a real vocabulary,
        // decoding returns the token ids as space-separated strings.
        return string.Join(" ", tokens);
    }

    /// <inheritdoc />
    public int CountTokens(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return 0;
        }

        return text.Split(Separator, StringSplitOptions.RemoveEmptyEntries).Length;
    }
}
