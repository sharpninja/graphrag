// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Storage;

namespace GraphRag.Cache;

/// <summary>
/// The default configuration section for cache.
/// </summary>
public sealed record CacheConfig
{
    /// <summary>Gets the cache type to use. Builtin types include "json", "memory", and "none".</summary>
    public string Type { get; init; } = CacheType.Json;

    /// <summary>Gets the storage configuration to use for file-based caches such as JSON.</summary>
    public StorageConfig? Storage { get; init; }
}
