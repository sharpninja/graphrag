// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Llm;

namespace GraphRag.PromptTune.Generator;

/// <summary>
/// Creates an expert persona description from the detected domain.
/// </summary>
public sealed class PersonaGenerator : IPromptGenerator
{
    /// <inheritdoc/>
    public Task<string> GenerateAsync(ILlmCompletion model, IReadOnlyList<string> docs, CancellationToken ct)
    {
        // TODO: Generate persona from domain and docs via LLM.
        return Task.FromResult(string.Empty);
    }
}
