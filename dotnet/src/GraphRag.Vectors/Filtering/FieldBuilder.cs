// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Vectors.Filtering;

/// <summary>
/// Static helper for building field references used in filter expressions.
/// </summary>
/// <example>
/// <code>
/// var filter = FieldBuilder.Field("name") == "value";
/// </code>
/// </example>
public static class FieldBuilder
{
    /// <summary>
    /// Creates a <see cref="FieldRef"/> for the specified field name.
    /// </summary>
    /// <param name="name">The field name.</param>
    /// <returns>A <see cref="FieldRef"/> instance.</returns>
    public static FieldRef Field(string name) => new(name);
}
