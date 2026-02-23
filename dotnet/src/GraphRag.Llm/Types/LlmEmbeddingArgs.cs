// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm.Types;

/// <summary>
/// Arguments for an LLM embedding request.
/// </summary>
/// <param name="Input">The list of text inputs to embed.</param>
/// <param name="Dimensions">An optional number of dimensions for the embedding vectors.</param>
/// <param name="EncodingFormat">An optional encoding format.</param>
/// <param name="User">An optional user identifier.</param>
public sealed record LlmEmbeddingArgs(
    IReadOnlyList<string> Input,
    int? Dimensions = null,
    string? EncodingFormat = null,
    string? User = null);
