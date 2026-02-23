// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm.Types;

/// <summary>
/// The response from an LLM embedding request.
/// </summary>
/// <param name="Embeddings">The list of embedding vectors.</param>
/// <param name="Usage">Token usage information.</param>
public sealed record LlmEmbeddingResponse(
    IReadOnlyList<IReadOnlyList<float>> Embeddings,
    LlmUsage? Usage = null)
{
    /// <summary>
    /// Gets the first embedding vector, or an empty list if no embeddings are present.
    /// </summary>
    public IReadOnlyList<float> FirstEmbedding =>
        Embeddings.Count > 0 ? Embeddings[0] : Array.Empty<float>();
}
