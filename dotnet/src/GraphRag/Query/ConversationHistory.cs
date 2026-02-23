// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Query;

/// <summary>
/// Tracks the history of a multi-turn conversation.
/// </summary>
public class ConversationHistory
{
    private readonly List<QaTurn> turns = [];

    /// <summary>
    /// Gets the read-only list of conversation turns.
    /// </summary>
    public IReadOnlyList<QaTurn> Turns => turns;

    /// <summary>
    /// Adds a new question-and-answer turn to the conversation history.
    /// </summary>
    /// <param name="query">The user query.</param>
    /// <param name="answer">The response answer.</param>
    public void AddTurn(string query, string answer)
    {
        turns.Add(new QaTurn(query, answer));
    }
}
