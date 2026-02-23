// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Index.Operations;

/// <summary>
/// Extracts entities and relationships from text to form a knowledge graph.
/// </summary>
public interface IGraphExtractor
{
    /// <summary>
    /// Extracts a graph from the given text.
    /// </summary>
    /// <param name="text">The input text to extract from.</param>
    /// <param name="ct">A token to cancel the operation.</param>
    /// <returns>A <see cref="GraphExtractionResult"/> containing entities and relationships.</returns>
    Task<GraphExtractionResult> ExtractAsync(string text, CancellationToken ct);
}
