// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Cache;

/// <summary>
/// Builtin cache implementation types.
/// </summary>
public static class CacheType
{
    /// <summary>Gets the identifier for JSON file-based caching.</summary>
    public const string Json = "json";

    /// <summary>Gets the identifier for in-memory caching.</summary>
    public const string Memory = "memory";

    /// <summary>Gets the identifier for no-op caching.</summary>
    public const string Noop = "none";
}
