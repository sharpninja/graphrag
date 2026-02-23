// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;

using GraphRag.Storage;

namespace GraphRag.Tests.Integration.Storage;

public class FileStorageIntegrationTests : IDisposable
{
    private readonly string _tempDir;
    private readonly FileStorage _storage;

    public FileStorageIntegrationTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "graphrag_test_" + Guid.NewGuid().ToString("N"));
        _storage = new FileStorage(_tempDir);
    }

    [Fact]
    public async Task CreateChild_WriteRead_RoundTrips()
    {
        // Arrange
        var child = _storage.Child("subdir");
        const string key = "test.txt";
        const string content = "hello from child storage";

        // Act
        await child.SetAsync(key, content);
        var result = await child.GetAsync(key);

        // Assert
        result.Should().Be(content);
        (await _storage.GetAsync(key)).Should().BeNull("parent should not contain child's key");
    }

    [Fact]
    public async Task ClearAll_RemovesEverything()
    {
        // Arrange
        await _storage.SetAsync("file1.txt", "one");
        await _storage.SetAsync("file2.txt", "two");
        _storage.Keys().Should().HaveCount(2);

        // Act
        await _storage.ClearAsync();

        // Assert
        _storage.Keys().Should().BeEmpty();
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
