// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Vectors.Filtering;

/// <summary>
/// A filter expression that negates its inner expression.
/// </summary>
public class NotExpression : FilterExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NotExpression"/> class.
    /// </summary>
    /// <param name="expression">The expression to negate.</param>
    public NotExpression(FilterExpression expression)
    {
        Expression = expression;
    }

    /// <summary>
    /// Gets the inner expression.
    /// </summary>
    public FilterExpression Expression { get; }

    /// <inheritdoc/>
    public override bool Evaluate(object? obj) =>
        !Expression.Evaluate(obj);
}
