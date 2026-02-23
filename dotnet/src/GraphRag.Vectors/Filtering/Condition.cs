// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Collections;

namespace GraphRag.Vectors.Filtering;

/// <summary>
/// A leaf filter expression that compares a field value using an operator.
/// </summary>
public class Condition : FilterExpression
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Condition"/> class.
    /// </summary>
    /// <param name="field">The field name to compare.</param>
    /// <param name="op">The comparison operator.</param>
    /// <param name="value">The value to compare against.</param>
    public Condition(string field, string op, object? value)
    {
        Field = field;
        Op = op;
        Value = value;
    }

    /// <summary>
    /// Gets the field name.
    /// </summary>
    public string Field { get; }

    /// <summary>
    /// Gets the comparison operator.
    /// </summary>
    public string Op { get; }

    /// <summary>
    /// Gets the comparison value.
    /// </summary>
    public object? Value { get; }

    /// <inheritdoc/>
    public override bool Evaluate(object? obj)
    {
        if (obj is not IDictionary<string, object?> dict)
        {
            return false;
        }

        bool hasField = dict.TryGetValue(Field, out var fieldValue);

        if (Op == ComparisonOperator.Exists)
        {
            return hasField;
        }

        if (!hasField)
        {
            return false;
        }

        return Op switch
        {
            ComparisonOperator.Eq => Equals(fieldValue, Value),
            ComparisonOperator.Ne => !Equals(fieldValue, Value),
            ComparisonOperator.Gt => Compare(fieldValue, Value) > 0,
            ComparisonOperator.Gte => Compare(fieldValue, Value) >= 0,
            ComparisonOperator.Lt => Compare(fieldValue, Value) < 0,
            ComparisonOperator.Lte => Compare(fieldValue, Value) <= 0,
            ComparisonOperator.Contains => fieldValue?.ToString()?.Contains(Value?.ToString() ?? string.Empty, StringComparison.Ordinal) == true,
            ComparisonOperator.StartsWith => fieldValue?.ToString()?.StartsWith(Value?.ToString() ?? string.Empty, StringComparison.Ordinal) == true,
            ComparisonOperator.EndsWith => fieldValue?.ToString()?.EndsWith(Value?.ToString() ?? string.Empty, StringComparison.Ordinal) == true,
            ComparisonOperator.In => Value is IEnumerable values && Contains(values, fieldValue),
            ComparisonOperator.NotIn => Value is not IEnumerable notInValues || !Contains(notInValues, fieldValue),
            _ => false,
        };
    }

    private static int Compare(object? a, object? b)
    {
        if (a is IComparable comparableA && b is not null)
        {
            return comparableA.CompareTo(Convert.ChangeType(b, a.GetType(), System.Globalization.CultureInfo.InvariantCulture));
        }

        return 0;
    }

    private static bool Contains(IEnumerable collection, object? value)
    {
        foreach (var item in collection)
        {
            if (Equals(item, value))
            {
                return true;
            }
        }

        return false;
    }
}
