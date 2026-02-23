// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Cache;

/// <summary>
/// Provides a cache interface for reading and writing data by key.
/// </summary>
public interface ICache
{
    /// <summary>
    /// Get the cached value for the given key.
    /// </summary>
    /// <param name="key">The key to get the value for.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The cached value, or <c>null</c> if not found.</returns>
    Task<object?> GetAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the cached value for the given key.
    /// </summary>
    /// <param name="key">The key to set the value for.</param>
    /// <param name="value">The value to cache.</param>
    /// <param name="debugData">Optional debug data to store alongside the value.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetAsync(string key, object? value, Dictionary<string, object?>? debugData = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Return <c>true</c> if the given key exists in the cache.
    /// </summary>
    /// <param name="key">The key to check for.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns><c>true</c> if the key exists; otherwise, <c>false</c>.</returns>
    Task<bool> HasAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete the given key from the cache.
    /// </summary>
    /// <param name="key">The key to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clear all entries in the cache.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ClearAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a child cache instance.
    /// </summary>
    /// <param name="name">The name of the child cache.</param>
    /// <returns>The child cache instance.</returns>
    ICache Child(string name);
}
