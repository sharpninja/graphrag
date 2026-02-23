// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Text.RegularExpressions;

namespace GraphRag.Storage;

/// <summary>
/// Provides a storage interface for reading and writing data by key.
/// </summary>
public interface IStorage
{
    /// <summary>
    /// Find files in the storage using a regular expression pattern.
    /// </summary>
    /// <param name="filePattern">The regular expression pattern to use for finding files.</param>
    /// <returns>An enumerable of matching file keys.</returns>
    IEnumerable<string> Find(Regex filePattern);

    /// <summary>
    /// Get the value for the given key.
    /// </summary>
    /// <param name="key">The key to get the value for.</param>
    /// <param name="asBytes">Whether to return the value as bytes.</param>
    /// <param name="encoding">The encoding to use when decoding the value.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The value for the given key, or <c>null</c> if not found.</returns>
    Task<object?> GetAsync(string key, bool? asBytes = null, string? encoding = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Set the value for the given key.
    /// </summary>
    /// <param name="key">The key to set the value for.</param>
    /// <param name="value">The value to set.</param>
    /// <param name="encoding">The encoding to use when writing the value.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SetAsync(string key, object value, string? encoding = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Return <c>true</c> if the given key exists in the storage.
    /// </summary>
    /// <param name="key">The key to check for.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns><c>true</c> if the key exists; otherwise, <c>false</c>.</returns>
    Task<bool> HasAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete the given key from the storage.
    /// </summary>
    /// <param name="key">The key to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clear all entries in the storage.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ClearAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Create a child storage instance.
    /// </summary>
    /// <param name="name">The name of the child storage, or <c>null</c> to return this instance.</param>
    /// <returns>The child storage instance.</returns>
    IStorage Child(string? name);

    /// <summary>
    /// List all keys in the storage.
    /// </summary>
    /// <returns>A read-only list of keys.</returns>
    IReadOnlyList<string> Keys();

    /// <summary>
    /// Get the creation date for the given key.
    /// </summary>
    /// <param name="key">The key to get the creation date for.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The formatted creation date string.</returns>
    Task<string> GetCreationDateAsync(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get the formatted timestamp with the local time zone.
    /// </summary>
    /// <param name="timestamp">The UTC timestamp to format.</param>
    /// <returns>A string in the format "yyyy-MM-dd HH:mm:ss zzz".</returns>
    static string GetTimestampFormattedWithLocalTz(DateTime timestamp)
    {
        var local = timestamp.ToLocalTime();
        var offset = TimeZoneInfo.Local.GetUtcOffset(local);
        var sign = offset < TimeSpan.Zero ? "-" : "+";
        return $"{local:yyyy-MM-dd HH:mm:ss} {sign}{offset:hhmm}";
    }
}
