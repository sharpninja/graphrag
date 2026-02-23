// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Llm;

namespace GraphRag.PromptTune.Generator;

/// <summary>
/// Generates a community reporter role description using an LLM.
/// </summary>
public sealed class CommunityReporterRoleGenerator : IPromptGenerator
{
    /// <inheritdoc/>
    public Task<string> GenerateAsync(ILlmCompletion model, IReadOnlyList<string> docs, CancellationToken ct)
    {
        // TODO: Generate community reporter role via LLM.
        return Task.FromResult(string.Empty);
    }
}
