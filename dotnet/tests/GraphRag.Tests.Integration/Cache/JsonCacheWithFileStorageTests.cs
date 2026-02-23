// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;

using GraphRag.Cache;
using GraphRag.Storage;

namespace GraphRag.Tests.Integration.Cache;

public class JsonCacheWithFileStorageTests : IDisposable
{
    private readonly string _tempDir;
    private readonly JsonCache _cache;

    public JsonCacheWithFileStorageTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "graphrag_cache_test_" + Guid.NewGuid().ToString("N"));
        var storage = new FileStorage(_tempDir);
        _cache = new JsonCache(storage);
    }

    [Fact]
    public async Task SetGet_PersistsToFileStorage()
    {
        // Arrange
        const string key = "my_cache_key";
        const string value = "cached_value";

        // Act
        await _cache.SetAsync(key, value);
        var result = await _cache.GetAsync(key);

        // Assert
        result.Should().NotBeNull();
        result!.ToString().Should().Contain(value);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, recursive: true);
        }

        GC.SuppressFinalize(this);
    }
}
