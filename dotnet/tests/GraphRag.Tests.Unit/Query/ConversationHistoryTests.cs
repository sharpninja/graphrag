// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Query;

namespace GraphRag.Tests.Unit.Query;

/// <summary>
/// Unit tests for <see cref="ConversationHistory"/>.
/// </summary>
public class ConversationHistoryTests
{
    [Fact]
    public void AddTurn_AddsTurn()
    {
        var history = new ConversationHistory();

        history.AddTurn("What is GraphRAG?", "A graph-based retrieval system.");

        history.Turns.Should().HaveCount(1);
        history.Turns[0].Query.Should().Be("What is GraphRAG?");
        history.Turns[0].Answer.Should().Be("A graph-based retrieval system.");
    }

    [Fact]
    public void Turns_ReturnsAllTurns()
    {
        var history = new ConversationHistory();
        history.AddTurn("Q1", "A1");
        history.AddTurn("Q2", "A2");
        history.AddTurn("Q3", "A3");

        history.Turns.Should().HaveCount(3);
        history.Turns[0].Query.Should().Be("Q1");
        history.Turns[1].Query.Should().Be("Q2");
        history.Turns[2].Query.Should().Be("Q3");
    }
}
