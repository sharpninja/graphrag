// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Vectors.Filtering;

/// <summary>
/// String constants for supported comparison operators.
/// </summary>
public static class ComparisonOperator
{
    /// <summary>Equal.</summary>
    public const string Eq = "eq";

    /// <summary>Not equal.</summary>
    public const string Ne = "ne";

    /// <summary>Greater than.</summary>
    public const string Gt = "gt";

    /// <summary>Greater than or equal.</summary>
    public const string Gte = "gte";

    /// <summary>Less than.</summary>
    public const string Lt = "lt";

    /// <summary>Less than or equal.</summary>
    public const string Lte = "lte";

    /// <summary>Contains.</summary>
    public const string Contains = "contains";

    /// <summary>Starts with.</summary>
    public const string StartsWith = "startswith";

    /// <summary>Ends with.</summary>
    public const string EndsWith = "endswith";

    /// <summary>In a set of values.</summary>
    public const string In = "in";

    /// <summary>Not in a set of values.</summary>
    public const string NotIn = "notin";

    /// <summary>Field exists.</summary>
    public const string Exists = "exists";
}
