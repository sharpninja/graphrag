// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm;

/// <summary>
/// Interface for tokenizing text into tokens and decoding tokens back to text.
/// </summary>
public interface ITokenizer
{
    /// <summary>
    /// Encodes the specified text into a list of token identifiers.
    /// </summary>
    /// <param name="text">The text to encode.</param>
    /// <returns>A read-only list of token identifiers.</returns>
    IReadOnlyList<int> Encode(string text);

    /// <summary>
    /// Decodes the specified token identifiers back into text.
    /// </summary>
    /// <param name="tokens">The token identifiers to decode.</param>
    /// <returns>The decoded text.</returns>
    string Decode(IReadOnlyList<int> tokens);

    /// <summary>
    /// Counts the number of tokens in the specified text.
    /// </summary>
    /// <param name="text">The text to count tokens for.</param>
    /// <returns>The number of tokens.</returns>
    int CountTokens(string text) => Encode(text).Count;
}
