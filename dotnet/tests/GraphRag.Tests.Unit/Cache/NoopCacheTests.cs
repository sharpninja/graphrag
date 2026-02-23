// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Cache;

namespace GraphRag.Tests.Unit.Cache;

/// <summary>
/// Unit tests for <see cref="NoopCache"/>.
/// </summary>
public class NoopCacheTests
{
    private readonly NoopCache _cache = new();

    [Fact]
    public async Task GetAsync_AlwaysReturnsNull()
    {
        await _cache.SetAsync("key1", "value1");
        var result = await _cache.GetAsync("key1");
        result.Should().BeNull();
    }

    [Fact]
    public async Task HasAsync_AlwaysReturnsFalse()
    {
        await _cache.SetAsync("key1", "value1");
        var result = await _cache.HasAsync("key1");
        result.Should().BeFalse();
    }

    [Fact]
    public async Task SetAsync_DoesNotThrow()
    {
        var act = () => _cache.SetAsync("key1", "value1");
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public void Child_ReturnsSelf()
    {
        var child = _cache.Child("sub");
        child.Should().BeSameAs(_cache);
    }
}
