// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.PromptTune.PromptCreator;

/// <summary>
/// Creates the graph extraction prompt from tuned artifacts.
/// </summary>
public static class ExtractGraphPromptCreator
{
    /// <summary>
    /// Creates the extraction prompt using the provided tuning results.
    /// </summary>
    /// <param name="entityTypes">The detected entity types.</param>
    /// <param name="examples">The generated extraction examples.</param>
    /// <param name="language">The detected language.</param>
    /// <returns>The formatted extraction prompt.</returns>
    public static string CreateExtractGraphPrompt(string entityTypes, string examples, string language)
    {
        // TODO: Compose the full graph extraction prompt template.
        return string.Empty;
    }
}
