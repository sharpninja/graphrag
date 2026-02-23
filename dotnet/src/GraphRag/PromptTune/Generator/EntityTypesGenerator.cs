// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Llm;

namespace GraphRag.PromptTune.Generator;

/// <summary>
/// Identifies relevant entity types from the input documents using an LLM.
/// </summary>
public sealed class EntityTypesGenerator : IPromptGenerator
{
    /// <inheritdoc/>
    public Task<string> GenerateAsync(ILlmCompletion model, IReadOnlyList<string> docs, CancellationToken ct)
    {
        // TODO: Identify entity types via LLM.
        return Task.FromResult(string.Empty);
    }
}
