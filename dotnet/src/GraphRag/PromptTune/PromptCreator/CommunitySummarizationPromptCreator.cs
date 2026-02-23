// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.PromptTune.PromptCreator;

/// <summary>
/// Creates the community summarization prompt from tuned artifacts.
/// </summary>
public static class CommunitySummarizationPromptCreator
{
    /// <summary>
    /// Creates the community summarization prompt.
    /// </summary>
    /// <param name="role">The community reporter role.</param>
    /// <param name="rating">The report rating criteria.</param>
    /// <param name="language">The detected language.</param>
    /// <returns>The formatted community summarization prompt.</returns>
    public static string CreateCommunitySummarizationPrompt(string role, string rating, string language)
    {
        // TODO: Compose the community summarization prompt template.
        return string.Empty;
    }
}
