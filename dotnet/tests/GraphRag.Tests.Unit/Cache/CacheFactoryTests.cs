// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Cache;
using GraphRag.Storage;

namespace GraphRag.Tests.Unit.Cache;

/// <summary>
/// Unit tests for <see cref="CacheFactory"/>.
/// </summary>
public class CacheFactoryTests
{
    private readonly CacheFactory _factory = new();

    [Fact]
    public void CreateCache_JsonType_CreatesJsonCache()
    {
        var config = new CacheConfig { Type = CacheType.Json };
        var storage = new MemoryStorage();
        var cache = _factory.CreateCache(config, storage);
        cache.Should().BeOfType<JsonCache>();
    }

    [Fact]
    public void CreateCache_MemoryType_CreatesMemoryCache()
    {
        var config = new CacheConfig { Type = CacheType.Memory };
        var cache = _factory.CreateCache(config);
        cache.Should().BeOfType<MemoryCache>();
    }

    [Fact]
    public void CreateCache_NoopType_CreatesNoopCache()
    {
        var config = new CacheConfig { Type = CacheType.Noop };
        var cache = _factory.CreateCache(config);
        cache.Should().BeOfType<NoopCache>();
    }

    [Fact]
    public void CreateCache_UnknownType_Throws()
    {
        var config = new CacheConfig { Type = "unknown" };
        var act = () => _factory.CreateCache(config);
        act.Should().Throw<InvalidOperationException>();
    }
}
