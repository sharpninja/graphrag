// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Index.Operations;

/// <summary>
/// Generates vector embeddings for text.
/// </summary>
public interface ITextEmbedder
{
    /// <summary>
    /// Generates an embedding vector for the given text.
    /// </summary>
    /// <param name="text">The text to embed.</param>
    /// <param name="ct">A token to cancel the operation.</param>
    /// <returns>A list of floats representing the embedding vector.</returns>
    Task<IReadOnlyList<float>> EmbedAsync(string text, CancellationToken ct);
}
