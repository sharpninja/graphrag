// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Common.Factories;
using GraphRag.Storage;

namespace GraphRag.Cache;

/// <summary>
/// A factory class for creating <see cref="ICache"/> instances by strategy name.
/// Supports lazy registration of builtin cache types.
/// </summary>
public class CacheFactory : ServiceFactory<ICache>
{
    /// <summary>
    /// Create a cache instance based on the given configuration, lazily registering builtin types.
    /// </summary>
    /// <param name="config">The cache configuration to use, or <c>null</c> for defaults.</param>
    /// <param name="storage">An optional storage instance to use for file-based caches.</param>
    /// <returns>The created cache implementation.</returns>
    /// <exception cref="InvalidOperationException">If the cache type is not registered and not a known builtin.</exception>
    public ICache CreateCache(CacheConfig? config = null, IStorage? storage = null)
    {
        config ??= new CacheConfig();
        var strategy = config.Type;

        if (storage is null && config.Storage is not null)
        {
            var storageFactory = new StorageFactory();
            storage = storageFactory.CreateStorage(config.Storage);
        }

        if (!Contains(strategy))
        {
            RegisterBuiltin(strategy);
        }

        var args = new Dictionary<string, object?>
        {
            ["type"] = config.Type,
            ["storage"] = storage,
        };

        return Create(strategy, args);
    }

    private void RegisterBuiltin(string strategy)
    {
        switch (strategy)
        {
            case CacheType.Json:
                Register(CacheType.Json, args =>
                    new JsonCache((IStorage)args["storage"]!));
                break;

            case CacheType.Memory:
                Register(CacheType.Memory, _ => new MemoryCache());
                break;

            case CacheType.Noop:
                Register(CacheType.Noop, _ => new NoopCache());
                break;

            default:
                var registered = string.Join(", ", Keys);
                throw new InvalidOperationException(
                    $"CacheConfig.Type '{strategy}' is not registered in the CacheFactory. Registered types: {registered}.");
        }
    }
}
