// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Input;
using GraphRag.Storage;

namespace GraphRag.Tests.Unit.Input;

/// <summary>
/// Unit tests for <see cref="JsonFileReader"/>.
/// </summary>
public class JsonFileReaderTests
{
    [Fact]
    public async Task ReadFilesAsync_WithJsonArray_ReturnsDocuments()
    {
        var storage = new MemoryStorage();
        var json = """
            [
                {"id": "1", "title": "Doc1", "text": "First document"},
                {"id": "2", "title": "Doc2", "text": "Second document"}
            ]
            """;
        await storage.SetAsync("data.json", json);

        var reader = new JsonFileReader(storage, idColumn: "id", titleColumn: "title", textColumn: "text");

        var docs = await reader.ReadFilesAsync();

        docs.Should().HaveCount(2);
        docs[0].Id.Should().Be("1");
        docs[0].Title.Should().Be("Doc1");
        docs[0].Text.Should().Be("First document");
        docs[1].Id.Should().Be("2");
    }

    [Fact]
    public async Task ReadFilesAsync_WithSingleObject_ReturnsDocument()
    {
        var storage = new MemoryStorage();
        var json = """{"id": "single", "title": "Only", "text": "Single doc"}""";
        await storage.SetAsync("single.json", json);

        var reader = new JsonFileReader(storage, idColumn: "id", titleColumn: "title", textColumn: "text");

        var docs = await reader.ReadFilesAsync();

        docs.Should().HaveCount(1);
        docs[0].Id.Should().Be("single");
        docs[0].Text.Should().Be("Single doc");
    }
}
