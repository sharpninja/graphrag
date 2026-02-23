// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Cache;

namespace GraphRag.Tests.Unit.Cache;

/// <summary>
/// Unit tests for <see cref="CacheKeyHelper"/>.
/// </summary>
public class CacheKeyHelperTests
{
    [Fact]
    public void CreateCacheKey_SameArgs_SameKey()
    {
        var args1 = new Dictionary<string, object?> { ["a"] = "1", ["b"] = "2" };
        var args2 = new Dictionary<string, object?> { ["a"] = "1", ["b"] = "2" };

        var key1 = CacheKeyHelper.CreateCacheKey(args1);
        var key2 = CacheKeyHelper.CreateCacheKey(args2);

        key1.Should().Be(key2);
    }

    [Fact]
    public void CreateCacheKey_DifferentArgs_DifferentKey()
    {
        var args1 = new Dictionary<string, object?> { ["a"] = "1" };
        var args2 = new Dictionary<string, object?> { ["a"] = "2" };

        var key1 = CacheKeyHelper.CreateCacheKey(args1);
        var key2 = CacheKeyHelper.CreateCacheKey(args2);

        key1.Should().NotBe(key2);
    }
}
