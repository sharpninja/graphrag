// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.PromptTune;

/// <summary>
/// Document selection strategies for prompt tuning.
/// </summary>
public static class DocSelectionType
{
    /// <summary>Use all available documents.</summary>
    public const string All = "all";

    /// <summary>Use a random sample of documents.</summary>
    public const string Random = "random";

    /// <summary>Use the top-ranked documents.</summary>
    public const string Top = "top";

    /// <summary>Automatically select the best strategy.</summary>
    public const string Auto = "auto";
}
