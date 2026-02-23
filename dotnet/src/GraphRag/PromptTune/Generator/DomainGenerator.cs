// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Llm;

namespace GraphRag.PromptTune.Generator;

/// <summary>
/// Detects the domain of the input documents using an LLM.
/// </summary>
public sealed class DomainGenerator : IPromptGenerator
{
    /// <inheritdoc/>
    public Task<string> GenerateAsync(ILlmCompletion model, IReadOnlyList<string> docs, CancellationToken ct)
    {
        // TODO: Send docs to LLM to detect domain.
        return Task.FromResult("General");
    }
}
