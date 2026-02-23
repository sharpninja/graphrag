// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Query;

/// <summary>
/// Represents the result of a context building operation.
/// </summary>
/// <param name="ContextChunks">The list of context text chunks.</param>
/// <param name="ContextRecords">A dictionary of named context records.</param>
/// <param name="LlmCalls">The number of LLM calls made during context building.</param>
/// <param name="PromptTokens">The total number of prompt tokens used.</param>
/// <param name="OutputTokens">The total number of output tokens generated.</param>
public sealed record ContextBuilderResult(
    IReadOnlyList<string> ContextChunks,
    Dictionary<string, object?> ContextRecords,
    int LlmCalls = 0,
    int PromptTokens = 0,
    int OutputTokens = 0);
