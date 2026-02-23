// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Collections.Concurrent;

namespace GraphRag.Cache;

/// <summary>
/// In-memory cache implementation backed by a <see cref="ConcurrentDictionary{TKey, TValue}"/>.
/// </summary>
public class MemoryCache : ICache
{
    private readonly ConcurrentDictionary<string, object?> _cache = new();

    /// <inheritdoc/>
    public Task<object?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.TryGetValue(key, out var value);
        return Task.FromResult(value);
    }

    /// <inheritdoc/>
    public Task SetAsync(string key, object? value, Dictionary<string, object?>? debugData = null, CancellationToken cancellationToken = default)
    {
        _cache[key] = value;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<bool> HasAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_cache.ContainsKey(key));
    }

    /// <inheritdoc/>
    public Task DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        _cache.Clear();
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public ICache Child(string name)
    {
        return new MemoryCache();
    }
}
