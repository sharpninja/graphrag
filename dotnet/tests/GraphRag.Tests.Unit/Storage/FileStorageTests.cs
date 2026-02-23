// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Text.RegularExpressions;
using FluentAssertions;
using GraphRag.Storage;

namespace GraphRag.Tests.Unit.Storage;

/// <summary>
/// Unit tests for <see cref="FileStorage"/>.
/// </summary>
public class FileStorageTests : IDisposable
{
    private readonly string _tempDir;
    private readonly FileStorage _storage;

    public FileStorageTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "graphrag_test_" + Guid.NewGuid().ToString("N"));
        _storage = new FileStorage(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, recursive: true);
        }

        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task SetAsync_CreatesFile_GetAsync_ReadsItBack()
    {
        await _storage.SetAsync("test.txt", "hello world");
        var result = await _storage.GetAsync("test.txt");
        result.Should().Be("hello world");
    }

    [Fact]
    public async Task SetAsync_WithByteArray_WritesBinary()
    {
        byte[] data = [0x01, 0x02, 0x03];
        await _storage.SetAsync("binary.dat", data);

        var result = await _storage.GetAsync("binary.dat", asBytes: true);
        result.Should().BeOfType<byte[]>();
        ((byte[])result!).Should().Equal(data);
    }

    [Fact]
    public async Task HasAsync_ExistingFile_ReturnsTrue()
    {
        await _storage.SetAsync("exists.txt", "content");
        var result = await _storage.HasAsync("exists.txt");
        result.Should().BeTrue();
    }

    [Fact]
    public async Task HasAsync_MissingFile_ReturnsFalse()
    {
        var result = await _storage.HasAsync("missing.txt");
        result.Should().BeFalse();
    }

    [Fact]
    public async Task DeleteAsync_RemovesFile()
    {
        await _storage.SetAsync("toDelete.txt", "content");
        await _storage.DeleteAsync("toDelete.txt");
        var result = await _storage.HasAsync("toDelete.txt");
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ClearAsync_RemovesAllFiles()
    {
        await _storage.SetAsync("a.txt", "1");
        await _storage.SetAsync("b.txt", "2");
        await _storage.ClearAsync();
        _storage.Keys().Should().BeEmpty();
    }

    [Fact]
    public async Task Find_WithRegex_ReturnsMatchingFilenames()
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
    public async Task Keys_ReturnsFilenames()
    {
        await _storage.SetAsync("file1.txt", "1");
        await _storage.SetAsync("file2.txt", "2");

        var keys = _storage.Keys();
        keys.Should().HaveCount(2);
        keys.Should().Contain("file1.txt");
        keys.Should().Contain("file2.txt");
    }

    [Fact]
    public void Child_CreatesSubdirectoryStorage()
    {
        var child = _storage.Child("subdir");
        child.Should().BeOfType<FileStorage>();
        child.Should().NotBeSameAs(_storage);
    }

    [Fact]
    public async Task GetCreationDateAsync_ReturnsFormattedDateString()
    {
        await _storage.SetAsync("dated.txt", "content");
        var date = await _storage.GetCreationDateAsync("dated.txt");

        // Should match pattern like "2025-01-15 10:30:45 +0000"
        date.Should().MatchRegex(@"\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2} [+-]\d{4}");
    }
}
