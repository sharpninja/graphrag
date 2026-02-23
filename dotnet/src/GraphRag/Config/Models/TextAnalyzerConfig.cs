// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Config.Enums;

namespace GraphRag.Config.Models;

/// <summary>
/// Configuration for the text analyzer used in NLP-based graph extraction.
/// </summary>
public sealed record TextAnalyzerConfig
{
    /// <summary>Gets the noun phrase extractor type.</summary>
    public string ExtractorType { get; init; } = NounPhraseExtractorType.RegexEnglish;

    /// <summary>Gets the spaCy model name to use.</summary>
    public string ModelName { get; init; } = "en_core_web_md";

    /// <summary>Gets the maximum word length to consider.</summary>
    public int MaxWordLength { get; init; } = 15;

    /// <summary>Gets the word delimiter.</summary>
    public string WordDelimiter { get; init; } = " ";

    /// <summary>Gets a value indicating whether to include named entities.</summary>
    public bool IncludeNamedEntities { get; init; } = true;

    /// <summary>Gets the list of nouns to exclude.</summary>
    public IReadOnlyList<string>? ExcludeNouns { get; init; }

    /// <summary>Gets the entity tags to exclude.</summary>
    public IReadOnlyList<string> ExcludeEntityTags { get; init; } = ["DATE"];

    /// <summary>Gets the part-of-speech tags to exclude.</summary>
    public IReadOnlyList<string> ExcludePosTags { get; init; } = ["DET", "PRON", "INTJ", "X"];

    /// <summary>Gets the noun phrase part-of-speech tags.</summary>
    public IReadOnlyList<string> NounPhraseTags { get; init; } = ["PROPN", "NOUN"];

    /// <summary>Gets the noun phrase grammar rules.</summary>
    public IReadOnlyDictionary<string, string>? NounPhraseGrammars { get; init; }
}
