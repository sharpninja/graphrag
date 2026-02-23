// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Vectors.Filtering;

namespace GraphRag.Tests.Unit.Vectors.Filtering;

/// <summary>
/// Unit tests for <see cref="FieldRef"/>.
/// </summary>
public class FieldRefTests
{
    [Fact]
    public void EqualOperator_CreatesCondition()
    {
        var field = new FieldRef("status");
        var condition = field == "active";

        condition.Field.Should().Be("status");
        condition.Op.Should().Be(ComparisonOperator.Eq);
        condition.Value.Should().Be("active");
    }

    [Fact]
    public void GreaterThan_CreatesCondition()
    {
        var field = new FieldRef("score");
        var condition = field > 10;

        condition.Field.Should().Be("score");
        condition.Op.Should().Be(ComparisonOperator.Gt);
        condition.Value.Should().Be(10);
    }

    [Fact]
    public void Contains_CreatesCondition()
    {
        var field = new FieldRef("description");
        var condition = field.Contains("graph");

        condition.Field.Should().Be("description");
        condition.Op.Should().Be(ComparisonOperator.Contains);
        condition.Value.Should().Be("graph");
    }
}
