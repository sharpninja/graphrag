// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Llm;

namespace GraphRag.PromptTune.Generator;

/// <summary>
/// Generates entity-relationship extraction examples using an LLM.
/// </summary>
public sealed class EntityRelationshipGenerator : IPromptGenerator
{
    /// <inheritdoc/>
    public Task<string> GenerateAsync(ILlmCompletion model, IReadOnlyList<string> docs, CancellationToken ct)
    {
        // TODO: Generate entity-relationship examples via LLM.
        return Task.FromResult(string.Empty);
    }
}
