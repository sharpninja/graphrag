// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Vectors.Filtering;

namespace GraphRag.Tests.Unit.Vectors.Filtering;

/// <summary>
/// Unit tests for <see cref="Condition"/>.
/// </summary>
public class ConditionTests
{
    [Fact]
    public void Evaluate_Eq_Matches()
    {
        var condition = new Condition("name", ComparisonOperator.Eq, "alice");
        var data = new Dictionary<string, object?> { ["name"] = "alice" };
        condition.Evaluate(data).Should().BeTrue();
    }

    [Fact]
    public void Evaluate_Eq_DoesNotMatch()
    {
        var condition = new Condition("name", ComparisonOperator.Eq, "alice");
        var data = new Dictionary<string, object?> { ["name"] = "bob" };
        condition.Evaluate(data).Should().BeFalse();
    }

    [Fact]
    public void Evaluate_Gt_Matches()
    {
        var condition = new Condition("age", ComparisonOperator.Gt, 18);
        var data = new Dictionary<string, object?> { ["age"] = 25 };
        condition.Evaluate(data).Should().BeTrue();
    }

    [Fact]
    public void Evaluate_Contains_Matches()
    {
        var condition = new Condition("text", ComparisonOperator.Contains, "world");
        var data = new Dictionary<string, object?> { ["text"] = "hello world" };
        condition.Evaluate(data).Should().BeTrue();
    }

    [Fact]
    public void Evaluate_In_Matches()
    {
        var condition = new Condition("color", ComparisonOperator.In, new object[] { "red", "green", "blue" });
        var data = new Dictionary<string, object?> { ["color"] = "green" };
        condition.Evaluate(data).Should().BeTrue();
    }

    [Fact]
    public void And_CombinesTwoConditions()
    {
        var left = new Condition("a", ComparisonOperator.Eq, 1);
        var right = new Condition("b", ComparisonOperator.Eq, 2);
        var combined = left & right;

        var data = new Dictionary<string, object?> { ["a"] = 1, ["b"] = 2 };
        combined.Evaluate(data).Should().BeTrue();
    }

    [Fact]
    public void Or_CombinesTwoConditions()
    {
        var left = new Condition("a", ComparisonOperator.Eq, 1);
        var right = new Condition("b", ComparisonOperator.Eq, 2);
        var combined = left | right;

        var data = new Dictionary<string, object?> { ["a"] = 1, ["b"] = 999 };
        combined.Evaluate(data).Should().BeTrue();
    }

    [Fact]
    public void Not_InvertsCondition()
    {
        var condition = new Condition("x", ComparisonOperator.Eq, 1);
        var negated = ~condition;

        var data = new Dictionary<string, object?> { ["x"] = 2 };
        negated.Evaluate(data).Should().BeTrue();
    }
}
