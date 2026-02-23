// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Llm;

namespace GraphRag.PromptTune.Generator;

/// <summary>
/// Generates a prompt-tuning artifact from documents using an LLM.
/// </summary>
public interface IPromptGenerator
{
    /// <summary>
    /// Generates a result string from the provided documents.
    /// </summary>
    /// <param name="model">The LLM completion provider.</param>
    /// <param name="docs">The input documents.</param>
    /// <param name="ct">A token to cancel the operation.</param>
    /// <returns>The generated result.</returns>
    Task<string> GenerateAsync(ILlmCompletion model, IReadOnlyList<string> docs, CancellationToken ct);
}
