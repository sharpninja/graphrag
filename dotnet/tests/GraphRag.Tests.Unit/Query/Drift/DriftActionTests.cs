// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Query.StructuredSearch.Drift;

namespace GraphRag.Tests.Unit.Query.Drift;

/// <summary>
/// Unit tests for <see cref="DriftAction"/>.
/// </summary>
public class DriftActionTests
{
    [Fact]
    public void Record_Equality_Works()
    {
        var a = new DriftAction("query", "answer", 0.95);
        var b = new DriftAction("query", "answer", 0.95);
        var c = new DriftAction("other", "answer", 0.95);

        a.Should().Be(b);
        a.Should().NotBe(c);
    }
}
