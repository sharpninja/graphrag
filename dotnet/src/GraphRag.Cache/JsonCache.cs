// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Text.Json;

using GraphRag.Storage;

namespace GraphRag.Cache;

/// <summary>
/// JSON file-based cache implementation backed by an <see cref="IStorage"/> instance.
/// </summary>
public class JsonCache : ICache
{
    private readonly IStorage _storage;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonCache"/> class.
    /// </summary>
    /// <param name="storage">The storage instance to use for persistence.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="storage"/> is <c>null</c>.</exception>
    public JsonCache(IStorage storage)
    {
        _storage = storage ?? throw new ArgumentNullException(nameof(storage));
    }

    /// <inheritdoc/>
    public async Task<object?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        if (!await HasAsync(key, cancellationToken).ConfigureAwait(false))
        {
            return null;
        }

        try
        {
            var raw = await _storage.GetAsync(key, cancellationToken: cancellationToken).ConfigureAwait(false);
            if (raw is not string json)
            {
                return null;
            }

            var data = JsonSerializer.Deserialize<Dictionary<string, object?>>(json);
            if (data is not null && data.TryGetValue("result", out var result))
            {
                return result;
            }

            return null;
        }
        catch (JsonException)
        {
            await _storage.DeleteAsync(key, cancellationToken).ConfigureAwait(false);
            return null;
        }
    }

    /// <inheritdoc/>
    public async Task SetAsync(string key, object? value, Dictionary<string, object?>? debugData = null, CancellationToken cancellationToken = default)
    {
        if (value is null)
        {
            return;
        }

        var data = new Dictionary<string, object?> { ["result"] = value };
        if (debugData is not null)
        {
            foreach (var kvp in debugData)
            {
                data[kvp.Key] = kvp.Value;
            }
        }

        var json = JsonSerializer.Serialize(data);
        await _storage.SetAsync(key, json, cancellationToken: cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<bool> HasAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _storage.HasAsync(key, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        if (await HasAsync(key, cancellationToken).ConfigureAwait(false))
        {
            await _storage.DeleteAsync(key, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async Task ClearAsync(CancellationToken cancellationToken = default)
    {
        await _storage.ClearAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public ICache Child(string name)
    {
        return new JsonCache(_storage.Child(name));
    }
}
