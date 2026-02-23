// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Query;

/// <summary>
/// Represents a single question-and-answer turn in a conversation.
/// </summary>
/// <param name="Query">The user query.</param>
/// <param name="Answer">The response answer.</param>
/// <param name="Role">The role of the query author.</param>
public sealed record QaTurn(
    string Query,
    string Answer,
    string Role = "user");
