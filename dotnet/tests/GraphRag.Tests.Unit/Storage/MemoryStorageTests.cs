// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Text.RegularExpressions;
using FluentAssertions;
using GraphRag.Storage;

namespace GraphRag.Tests.Unit.Storage;

/// <summary>
/// Unit tests for <see cref="MemoryStorage"/>.
/// </summary>
public class MemoryStorageTests
{
    private readonly MemoryStorage _storage = new();

    [Fact]
    public async Task GetAsync_MissingKey_ReturnsNull()
    {
        var result = await _storage.GetAsync("nonexistent");
        result.Should().BeNull();
    }

    [Fact]
    public async Task SetAsync_ThenGetAsync_RoundTripsCorrectly()
    {
        await _storage.SetAsync("key1", "value1");
        var result = await _storage.GetAsync("key1");
        result.Should().Be("value1");
    }

    [Fact]
    public async Task HasAsync_ExistingKey_ReturnsTrue()
    {
        await _storage.SetAsync("key1", "value1");
        var result = await _storage.HasAsync("key1");
        result.Should().BeTrue();
    }

    [Fact]
    public async Task HasAsync_MissingKey_ReturnsFalse()
    {
        var result = await _storage.HasAsync("nonexistent");
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_RemovesKey()
    {
        await _storage.SetAsync("key1", "value1");
        await _storage.DeleteAsync("key1");
        var result = await _storage.HasAsync("key1");
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ClearAsync_RemovesAllKeys()
    {
        await _storage.SetAsync("key1", "value1");
        await _storage.SetAsync("key2", "value2");
        await _storage.ClearAsync();
        _storage.Keys().Should().BeEmpty();
    }

    [Fact]
    public async Task Find_WithRegex_ReturnsMatchingKeys()
    {
        await _storage.SetAsync("report.txt", "data");
        await _storage.SetAsync("report.csv", "data");
        await _storage.SetAsync("other.txt", "data");

        var matches = _storage.Find(new Regex(@"report\.")).ToList();
        matches.Should().HaveCount(2);
        matches.Should().Contain("report.txt");
        matches.Should().Contain("report.csv");
    }

    [Fact]
    public async Task Keys_ReturnsAllStoredKeys()
    {
        await _storage.SetAsync("a", "1");
        await _storage.SetAsync("b", "2");

        var keys = _storage.Keys();
        keys.Should().HaveCount(2);
        keys.Should().Contain("a");
        keys.Should().Contain("b");
    }

    [Fact]
    public void Child_ReturnsNewMemoryStorageInstance()
    {
        var child = _storage.Child("sub");
        child.Should().BeOfType<MemoryStorage>();
        child.Should().NotBeSameAs(_storage);
    }
}
