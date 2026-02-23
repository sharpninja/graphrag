// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Cache;
using GraphRag.Storage;

namespace GraphRag.Tests.Unit.Cache;

/// <summary>
/// Unit tests for <see cref="JsonCache"/>.
/// </summary>
public class JsonCacheTests
{
    private readonly MemoryStorage _storage = new();
    private readonly JsonCache _cache;

    public JsonCacheTests()
    {
        _cache = new JsonCache(_storage);
    }

    [Fact]
    public async Task GetAsync_MissingKey_ReturnsNull()
    {
        var result = await _cache.GetAsync("nonexistent");
        result.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_ThenGetAsync_RoundTrips()
    {
        await _cache.SetAsync("key1", "hello");
        var result = await _cache.GetAsync("key1");

        // JsonElement ToString returns the original value
        result.Should().NotBeNull();
        result!.ToString().Should().Be("hello");

        // Verify JSON was written to storage
        var raw = await _storage.GetAsync("key1");
        raw.Should().BeOfType<string>();
        ((string)raw!).Should().Contain("\"result\"");
    }

    [Fact]
    public async Task HasAsync_ExistingKey_ReturnsTrue()
    {
        await _cache.SetAsync("key1", "value1");
        var result = await _cache.HasAsync("key1");
        result.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAsync_RemovesFromStorage()
    {
        await _cache.SetAsync("key1", "value1");
        await _cache.DeleteAsync("key1");

        (await _cache.HasAsync("key1")).Should().BeFalse();
        (await _storage.HasAsync("key1")).Should().BeFalse();
    }

    [Fact]
    public async Task ClearAsync_ClearsStorage()
    {
        await _cache.SetAsync("key1", "value1");
        await _cache.SetAsync("key2", "value2");
        await _cache.ClearAsync();

        _storage.Keys().Should().BeEmpty();
    }

    [Fact]
    public void Child_ReturnsJsonCacheWithChildStorage()
    {
        var child = _cache.Child("sub");
        child.Should().BeOfType<JsonCache>();
        child.Should().NotBeSameAs(_cache);
    }
}
