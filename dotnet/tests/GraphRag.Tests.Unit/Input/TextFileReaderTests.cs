// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Input;
using GraphRag.Storage;

namespace GraphRag.Tests.Unit.Input;

/// <summary>
/// Unit tests for <see cref="TextFileReader"/>.
/// </summary>
public class TextFileReaderTests
{
    [Fact]
    public async Task ReadFilesAsync_WithTextFiles_ReturnsDocuments()
    {
        var storage = new MemoryStorage();
        await storage.SetAsync("doc1.txt", "Hello world");
        await storage.SetAsync("doc2.txt", "Second document");

        var reader = new TextFileReader(storage);

        var docs = await reader.ReadFilesAsync();

        docs.Should().HaveCount(2);
        docs.Should().Contain(d => d.Text == "Hello world");
        docs.Should().Contain(d => d.Text == "Second document");
        docs.Should().OnlyContain(d => !string.IsNullOrEmpty(d.Id));
    }

    [Fact]
    public async Task ReadFilesAsync_NoMatchingFiles_ReturnsEmpty()
    {
        var storage = new MemoryStorage();
        await storage.SetAsync("data.csv", "a,b,c");

        var reader = new TextFileReader(storage);

        var docs = await reader.ReadFilesAsync();

        docs.Should().BeEmpty();
    }
}
