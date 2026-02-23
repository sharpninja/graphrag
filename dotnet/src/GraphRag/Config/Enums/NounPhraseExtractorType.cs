// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Config.Enums;

/// <summary>
/// Available noun phrase extractor types.
/// </summary>
public static class NounPhraseExtractorType
{
    /// <summary>Gets the identifier for regex-based English extraction.</summary>
    public const string RegexEnglish = "regex_english";

    /// <summary>Gets the identifier for syntactic parser extraction.</summary>
    public const string Syntactic = "syntactic_parser";

    /// <summary>Gets the identifier for context-free grammar extraction.</summary>
    public const string Cfg = "cfg";
}
