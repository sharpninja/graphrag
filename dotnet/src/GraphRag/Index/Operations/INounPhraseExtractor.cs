// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Index.Operations;

/// <summary>
/// Extracts noun phrases from text using NLP techniques.
/// </summary>
public interface INounPhraseExtractor
{
    /// <summary>
    /// Extracts noun phrases from the given text.
    /// </summary>
    /// <param name="text">The text to extract noun phrases from.</param>
    /// <returns>A list of extracted noun phrases.</returns>
    IReadOnlyList<string> Extract(string text);
}
