// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Llm.Types;

namespace GraphRag.Llm;

/// <summary>
/// Interface for LLM embedding operations.
/// </summary>
public interface ILlmEmbedding
{
    /// <summary>
    /// Gets the tokenizer associated with this embedding provider.
    /// </summary>
    ITokenizer Tokenizer { get; }

    /// <summary>
    /// Sends an embedding request to the LLM asynchronously.
    /// </summary>
    /// <param name="args">The embedding request arguments.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The embedding response.</returns>
    Task<LlmEmbeddingResponse> EmbedAsync(LlmEmbeddingArgs args, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an embedding request to the LLM synchronously.
    /// </summary>
    /// <param name="args">The embedding request arguments.</param>
    /// <returns>The embedding response.</returns>
    LlmEmbeddingResponse Embed(LlmEmbeddingArgs args);
}
