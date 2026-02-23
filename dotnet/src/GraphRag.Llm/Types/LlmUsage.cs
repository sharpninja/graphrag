// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm.Types;

/// <summary>
/// Token usage information for an LLM request.
/// </summary>
/// <param name="PromptTokens">The number of tokens in the prompt.</param>
/// <param name="CompletionTokens">The number of tokens in the completion.</param>
/// <param name="TotalTokens">The total number of tokens used.</param>
public sealed record LlmUsage(
    int PromptTokens,
    int CompletionTokens,
    int TotalTokens);
