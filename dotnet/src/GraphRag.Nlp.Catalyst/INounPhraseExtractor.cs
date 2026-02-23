// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Nlp.Catalyst;

/// <summary>
/// Defines an interface for extracting noun phrases from text.
/// </summary>
/// <remarks>
/// This interface is defined locally until it is refactored into a shared
/// abstractions project (e.g., GraphRag.Common or GraphRag.Nlp.Abstractions).
/// </remarks>
public interface INounPhraseExtractor
{
    /// <summary>
    /// Extracts noun phrases from the given text.
    /// </summary>
    /// <param name="text">The input text to analyze.</param>
    /// <param name="ct">A token to cancel the operation.</param>
    /// <returns>A read-only list of extracted noun phrases.</returns>
    Task<IReadOnlyList<string>> ExtractNounPhrasesAsync(string text, CancellationToken ct = default);
}
