// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm.Types;

/// <summary>
/// Helper methods for working with metrics dictionaries.
/// </summary>
public static class MetricsHelper
{
    /// <summary>
    /// Creates a new empty metrics dictionary.
    /// </summary>
    /// <returns>A new <see cref="Dictionary{TKey, TValue}"/> for storing metrics.</returns>
    public static Dictionary<string, double> Create() => new();

    /// <summary>
    /// Creates a new metrics dictionary from the specified key-value pairs.
    /// </summary>
    /// <param name="values">The key-value pairs to initialize the dictionary with.</param>
    /// <returns>A new <see cref="Dictionary{TKey, TValue}"/> containing the specified metrics.</returns>
    public static Dictionary<string, double> Create(params (string Key, double Value)[] values)
    {
        var metrics = new Dictionary<string, double>(values.Length);
        foreach (var (key, value) in values)
        {
            metrics[key] = value;
        }

        return metrics;
    }
}
