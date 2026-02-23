// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Vectors.Filtering;

/// <summary>
/// Abstract base class for filter expressions used in vector store queries.
/// </summary>
public abstract class FilterExpression
{
    /// <summary>
    /// Evaluates the filter expression against the given object.
    /// </summary>
    /// <param name="obj">The object to evaluate against.</param>
    /// <returns><c>true</c> if the object matches the filter; otherwise, <c>false</c>.</returns>
    public abstract bool Evaluate(object? obj);

    /// <summary>
    /// Combines two filter expressions with a logical AND.
    /// </summary>
    /// <param name="left">The left-hand expression.</param>
    /// <param name="right">The right-hand expression.</param>
    /// <returns>An <see cref="AndExpression"/> combining both expressions.</returns>
    public static FilterExpression operator &(FilterExpression left, FilterExpression right) =>
        new AndExpression(left, right);

    /// <summary>
    /// Combines two filter expressions with a logical OR.
    /// </summary>
    /// <param name="left">The left-hand expression.</param>
    /// <param name="right">The right-hand expression.</param>
    /// <returns>An <see cref="OrExpression"/> combining both expressions.</returns>
    public static FilterExpression operator |(FilterExpression left, FilterExpression right) =>
        new OrExpression(left, right);

    /// <summary>
    /// Negates a filter expression.
    /// </summary>
    /// <param name="expression">The expression to negate.</param>
    /// <returns>A <see cref="NotExpression"/> wrapping the expression.</returns>
    public static FilterExpression operator ~(FilterExpression expression) =>
        new NotExpression(expression);
}
