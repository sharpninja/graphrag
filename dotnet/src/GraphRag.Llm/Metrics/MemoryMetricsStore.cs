// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Collections.Concurrent;

namespace GraphRag.Llm.Metrics;

/// <summary>
/// An in-memory metrics store backed by a <see cref="ConcurrentDictionary{TKey, TValue}"/>.
/// </summary>
public sealed class MemoryMetricsStore : IMetricsStore
{
    private readonly ConcurrentDictionary<string, Dictionary<string, double>> _store = new();

    /// <inheritdoc />
    public void Record(string key, Dictionary<string, double> metrics)
    {
        _store[key] = metrics;
    }

    /// <inheritdoc />
    public IReadOnlyDictionary<string, Dictionary<string, double>> GetAll()
    {
        return _store;
    }
}
