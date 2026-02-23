// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Llm;

namespace GraphRag.PromptTune.Generator;

/// <summary>
/// Generates community report rating criteria using an LLM.
/// </summary>
public sealed class CommunityReportRatingGenerator : IPromptGenerator
{
    /// <inheritdoc/>
    public Task<string> GenerateAsync(ILlmCompletion model, IReadOnlyList<string> docs, CancellationToken ct)
    {
        // TODO: Generate community report rating criteria via LLM.
        return Task.FromResult(string.Empty);
    }
}
