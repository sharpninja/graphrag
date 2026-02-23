// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Cache;

/// <summary>
/// A no-op cache implementation that does nothing. Useful for testing or disabling caching.
/// </summary>
public class NoopCache : ICache
{
    /// <inheritdoc/>
    public Task<object?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<object?>(null);
    }

    /// <inheritdoc/>
    public Task SetAsync(string key, object? value, Dictionary<string, object?>? debugData = null, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task<bool> HasAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(false);
    }

    /// <inheritdoc/>
    public Task DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public ICache Child(string name)
    {
        return this;
    }
}
