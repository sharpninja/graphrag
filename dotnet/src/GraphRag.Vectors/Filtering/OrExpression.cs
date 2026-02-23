// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Vectors.Filtering;

/// <summary>
/// A composite filter expression that requires at least one child expression to match.
/// </summary>
public class OrExpression : FilterExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OrExpression"/> class.
    /// </summary>
    /// <param name="left">The left-hand expression.</param>
    /// <param name="right">The right-hand expression.</param>
    public OrExpression(FilterExpression left, FilterExpression right)
    {
        Left = left;
        Right = right;
    }

    /// <summary>
    /// Gets the left-hand expression.
    /// </summary>
    public FilterExpression Left { get; }

    /// <summary>
    /// Gets the right-hand expression.
    /// </summary>
    public FilterExpression Right { get; }

    /// <inheritdoc/>
    public override bool Evaluate(object? obj) =>
        Left.Evaluate(obj) || Right.Evaluate(obj);
}
