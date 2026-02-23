// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm.Types;

/// <summary>
/// Arguments for an LLM completion request.
/// </summary>
/// <param name="Messages">The list of messages in the conversation.</param>
/// <param name="ResponseFormat">An optional type describing the expected response format.</param>
/// <param name="Temperature">An optional temperature value for sampling.</param>
/// <param name="TopP">An optional top-p value for nucleus sampling.</param>
/// <param name="MaxTokens">An optional maximum number of tokens to generate.</param>
/// <param name="Seed">An optional seed for deterministic generation.</param>
/// <param name="Stream">Whether to stream the response.</param>
/// <param name="User">An optional user identifier.</param>
/// <param name="Tools">An optional list of tool definitions.</param>
public sealed record LlmCompletionArgs(
    IReadOnlyList<LlmMessage> Messages,
    Type? ResponseFormat = null,
    double? Temperature = null,
    double? TopP = null,
    int? MaxTokens = null,
    int? Seed = null,
    bool? Stream = null,
    string? User = null,
    IReadOnlyList<object>? Tools = null);
