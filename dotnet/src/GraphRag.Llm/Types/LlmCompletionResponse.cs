// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm.Types;

/// <summary>
/// The response from an LLM completion request.
/// </summary>
/// <param name="Content">The text content of the response.</param>
/// <param name="FormattedResponse">The parsed structured response, if applicable.</param>
/// <param name="Usage">Token usage information.</param>
public sealed record LlmCompletionResponse(
    string? Content = null,
    object? FormattedResponse = null,
    LlmUsage? Usage = null);
