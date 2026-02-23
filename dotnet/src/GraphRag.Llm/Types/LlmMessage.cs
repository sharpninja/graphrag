// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm.Types;

/// <summary>
/// A message in an LLM conversation.
/// </summary>
/// <param name="Role">The role of the message sender (e.g., "system", "user", "assistant").</param>
/// <param name="Content">The content of the message.</param>
/// <param name="Name">An optional name for the message sender.</param>
/// <param name="ToolCalls">An optional list of tool calls.</param>
/// <param name="ToolCallId">An optional tool call identifier.</param>
public sealed record LlmMessage(
    string Role,
    string Content,
    string? Name = null,
    IReadOnlyList<object>? ToolCalls = null,
    string? ToolCallId = null);
