// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Llm.Types;

namespace GraphRag.Llm;

/// <summary>
/// Interface for LLM text completion operations.
/// </summary>
public interface ILlmCompletion
{
    /// <summary>
    /// Gets the tokenizer associated with this completion provider.
    /// </summary>
    ITokenizer Tokenizer { get; }

    /// <summary>
    /// Sends a completion request to the LLM asynchronously.
    /// </summary>
    /// <param name="args">The completion request arguments.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The completion response.</returns>
    Task<LlmCompletionResponse> CompleteAsync(LlmCompletionArgs args, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a completion request to the LLM synchronously.
    /// </summary>
    /// <param name="args">The completion request arguments.</param>
    /// <returns>The completion response.</returns>
    LlmCompletionResponse Complete(LlmCompletionArgs args);
}
