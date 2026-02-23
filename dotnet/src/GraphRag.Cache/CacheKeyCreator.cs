// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Common.Hasher;

namespace GraphRag.Cache;

/// <summary>
/// Delegate type for creating a cache key from input arguments.
/// </summary>
/// <param name="inputArgs">The input arguments for creating the cache key.</param>
/// <returns>The generated cache key.</returns>
public delegate string CacheKeyCreator(Dictionary<string, object?> inputArgs);

/// <summary>
/// Provides helper methods for creating cache keys.
/// </summary>
public static class CacheKeyHelper
{
    /// <summary>
    /// Create a cache key based on the input arguments by hashing them.
    /// </summary>
    /// <param name="inputArgs">The input arguments to hash into a cache key.</param>
    /// <returns>The generated cache key.</returns>
    public static string CreateCacheKey(Dictionary<string, object?> inputArgs)
    {
        return HashHelper.HashData(inputArgs);
    }
}
