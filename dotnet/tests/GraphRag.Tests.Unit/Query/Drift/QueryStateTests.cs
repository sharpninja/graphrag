// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Query.StructuredSearch.Drift;

namespace GraphRag.Tests.Unit.Query.Drift;

/// <summary>
/// Unit tests for <see cref="QueryState"/>.
/// </summary>
public class QueryStateTests
{
    [Fact]
    public void AddAction_TracksActions()
    {
        var state = new QueryState();
        var action = new DriftAction("sub-query", "answer", 0.95);

        state.AddAction(action);

        state.Actions.Should().HaveCount(1);
        state.Actions[0].Should().Be(action);
    }

    [Fact]
    public void Actions_ReturnsAllActions()
    {
        var state = new QueryState();
        state.AddAction(new DriftAction("q1", "a1", 0.9));
        state.AddAction(new DriftAction("q2", "a2", 0.8));
        state.AddAction(new DriftAction("q3"));

        state.Actions.Should().HaveCount(3);
        state.Actions[0].Query.Should().Be("q1");
        state.Actions[1].Query.Should().Be("q2");
        state.Actions[2].Query.Should().Be("q3");
        state.Actions[2].Answer.Should().BeNull();
        state.Actions[2].Score.Should().BeNull();
    }
}
