// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Cache;

namespace GraphRag.Tests.Unit.Cache;

/// <summary>
/// Unit tests for <see cref="MemoryCache"/>.
/// </summary>
public class MemoryCacheTests
{
    private readonly MemoryCache _cache = new();

    [Fact]
    public async Task GetAsync_MissingKey_ReturnsNull()
    {
        var result = await _cache.GetAsync("nonexistent");
        result.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_ThenGetAsync_RoundTrips()
    {
        await _cache.SetAsync("key1", "value1");
        var result = await _cache.GetAsync("key1");
        result.Should().Be("value1");
    }

    [Fact]
    public async Task HasAsync_ExistingKey_ReturnsTrue()
    {
        await _cache.SetAsync("key1", "value1");
        var result = await _cache.HasAsync("key1");
        result.Should().BeTrue();
    }

    [Fact]
    public async Task HasAsync_MissingKey_ReturnsFalse()
    {
        var result = await _cache.HasAsync("nonexistent");
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_RemovesKey()
    {
        await _cache.SetAsync("key1", "value1");
        await _cache.DeleteAsync("key1");
        var result = await _cache.HasAsync("key1");
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ClearAsync_RemovesAllKeys()
    {
        await _cache.SetAsync("key1", "value1");
        await _cache.SetAsync("key2", "value2");
        await _cache.ClearAsync();

        (await _cache.HasAsync("key1")).Should().BeFalse();
        (await _cache.HasAsync("key2")).Should().BeFalse();
    }

    [Fact]
    public void Child_ReturnsNewInstance()
    {
        var child = _cache.Child("sub");
        child.Should().BeOfType<MemoryCache>();
        child.Should().NotBeSameAs(_cache);
    }
}
