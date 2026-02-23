// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Vectors.Filtering;

/// <summary>
/// A fluent reference to a field used to build filter conditions.
/// </summary>
public class FieldRef
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FieldRef"/> class.
    /// </summary>
    /// <param name="name">The field name.</param>
    public FieldRef(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Gets the field name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Creates an equality condition.
    /// </summary>
    /// <param name="field">The field reference.</param>
    /// <param name="value">The value to compare against.</param>
    /// <returns>A <see cref="Condition"/> for the equality check.</returns>
    public static Condition operator ==(FieldRef field, object? value) =>
        new(field.Name, ComparisonOperator.Eq, value);

    /// <summary>
    /// Creates an inequality condition.
    /// </summary>
    /// <param name="field">The field reference.</param>
    /// <param name="value">The value to compare against.</param>
    /// <returns>A <see cref="Condition"/> for the inequality check.</returns>
    public static Condition operator !=(FieldRef field, object? value) =>
        new(field.Name, ComparisonOperator.Ne, value);

    /// <summary>
    /// Creates a greater-than condition.
    /// </summary>
    /// <param name="field">The field reference.</param>
    /// <param name="value">The value to compare against.</param>
    /// <returns>A <see cref="Condition"/> for the greater-than check.</returns>
    public static Condition operator >(FieldRef field, object? value) =>
        new(field.Name, ComparisonOperator.Gt, value);

    /// <summary>
    /// Creates a greater-than-or-equal condition.
    /// </summary>
    /// <param name="field">The field reference.</param>
    /// <param name="value">The value to compare against.</param>
    /// <returns>A <see cref="Condition"/> for the greater-than-or-equal check.</returns>
    public static Condition operator >=(FieldRef field, object? value) =>
        new(field.Name, ComparisonOperator.Gte, value);

    /// <summary>
    /// Creates a less-than condition.
    /// </summary>
    /// <param name="field">The field reference.</param>
    /// <param name="value">The value to compare against.</param>
    /// <returns>A <see cref="Condition"/> for the less-than check.</returns>
    public static Condition operator <(FieldRef field, object? value) =>
        new(field.Name, ComparisonOperator.Lt, value);

    /// <summary>
    /// Creates a less-than-or-equal condition.
    /// </summary>
    /// <param name="field">The field reference.</param>
    /// <param name="value">The value to compare against.</param>
    /// <returns>A <see cref="Condition"/> for the less-than-or-equal check.</returns>
    public static Condition operator <=(FieldRef field, object? value) =>
        new(field.Name, ComparisonOperator.Lte, value);

    /// <summary>
    /// Creates a contains condition.
    /// </summary>
    /// <param name="value">The substring to search for.</param>
    /// <returns>A <see cref="Condition"/> for the contains check.</returns>
    public Condition Contains(string value) =>
        new(Name, ComparisonOperator.Contains, value);

    /// <summary>
    /// Creates a starts-with condition.
    /// </summary>
    /// <param name="value">The prefix to match.</param>
    /// <returns>A <see cref="Condition"/> for the starts-with check.</returns>
    public Condition StartsWith(string value) =>
        new(Name, ComparisonOperator.StartsWith, value);

    /// <summary>
    /// Creates an ends-with condition.
    /// </summary>
    /// <param name="value">The suffix to match.</param>
    /// <returns>A <see cref="Condition"/> for the ends-with check.</returns>
    public Condition EndsWith(string value) =>
        new(Name, ComparisonOperator.EndsWith, value);

    /// <summary>
    /// Creates an in-set condition.
    /// </summary>
    /// <param name="values">The set of values to match against.</param>
    /// <returns>A <see cref="Condition"/> for the in-set check.</returns>
    public Condition In(IEnumerable<object> values) =>
        new(Name, ComparisonOperator.In, values);

    /// <summary>
    /// Creates a not-in-set condition.
    /// </summary>
    /// <param name="values">The set of values to exclude.</param>
    /// <returns>A <see cref="Condition"/> for the not-in-set check.</returns>
    public Condition NotIn(IEnumerable<object> values) =>
        new(Name, ComparisonOperator.NotIn, values);

    /// <summary>
    /// Creates a field-exists condition.
    /// </summary>
    /// <returns>A <see cref="Condition"/> that checks for field existence.</returns>
    public Condition Exists() =>
        new(Name, ComparisonOperator.Exists, null);

    /// <inheritdoc/>
    public override bool Equals(object? obj) =>
        obj is FieldRef other && Name == other.Name;

    /// <inheritdoc/>
    public override int GetHashCode() =>
        Name.GetHashCode(StringComparison.Ordinal);
}
