// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace GraphRag.Storage;

/// <summary>
/// In-memory storage implementation backed by a <see cref="ConcurrentDictionary{TKey, TValue}"/>.
/// </summary>
public class MemoryStorage : IStorage
{
    private readonly ConcurrentDictionary<string, object> _storage = new();

    /// <inheritdoc/>
    public IEnumerable<string> Find(Regex filePattern)
    {
        foreach (var key in _storage.Keys)
        {
            if (filePattern.IsMatch(key))
            {
                yield return key;
            }
        }
    }

    /// <inheritdoc/>
    public Task<object?> GetAsync(string key, bool? asBytes = null, string? encoding = null, CancellationToken cancellationToken = default)
    {
        _storage.TryGetValue(key, out var value);
        return Task.FromResult<object?>(value);
    }

    /// <inheritdoc/>
    public Task SetAsync(string key, object value, string? encoding = null, CancellationToken cancellationToken = default)
    {
        _storage[key] = value;
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<bool> HasAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_storage.ContainsKey(key));
    }

    /// <inheritdoc/>
    public Task DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        _storage.TryRemove(key, out _);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        _storage.Clear();
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public IStorage Child(string? name)
    {
        return new MemoryStorage();
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> Keys()
    {
        return _storage.Keys.ToList();
    }

    /// <inheritdoc/>
    public Task<string> GetCreationDateAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(IStorage.GetTimestampFormattedWithLocalTz(DateTime.UtcNow));
    }
}
