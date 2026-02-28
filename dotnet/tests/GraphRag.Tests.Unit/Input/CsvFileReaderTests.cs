// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Input;
using GraphRag.Storage;

namespace GraphRag.Tests.Unit.Input;

/// <summary>
/// Unit tests for <see cref="CsvFileReader"/>.
/// </summary>
public class CsvFileReaderTests
{
    [Fact]
    public async Task ReadFilesAsync_BasicCsv_ReturnsDocuments()
    {
        var storage = new MemoryStorage();
        await storage.SetAsync("data.csv", "id,title,text\n1,Hello,Hi how are you today?\n2,World,Fine thanks\n");

        var reader = new CsvFileReader(storage, idColumn: "id", titleColumn: "title", textColumn: "text");

        var docs = await reader.ReadFilesAsync();

        docs.Should().HaveCount(2);
        docs[0].Id.Should().Be("1");
        docs[0].Title.Should().Be("Hello");
        docs[0].Text.Should().Be("Hi how are you today?");
        docs[1].Id.Should().Be("2");
    }

    [Fact]
    public async Task ReadFilesAsync_MultilineQuotedField_PreservesInternalNewlines()
    {
        // Equivalent to Python test: test_csv_loader_preserves_multiline_fields
        var csvContent = "title,text\r\n\"Post 1\",\"Line one.\nLine two.\nLine three.\"\r\n\"Post 2\",\"Single line.\"\r\n";

        var storage = new MemoryStorage();
        await storage.SetAsync("input.csv", csvContent);

        var reader = new CsvFileReader(storage, titleColumn: "title", textColumn: "text");

        var docs = await reader.ReadFilesAsync();

        docs.Should().HaveCount(2);
        docs[0].Title.Should().Be("Post 1");
        docs[0].Text.Should().Be("Line one.\nLine two.\nLine three.");
        docs[1].Title.Should().Be("Post 2");
        docs[1].Text.Should().Be("Single line.");
    }

    [Fact]
    public async Task ReadFilesAsync_QuotedFieldWithComma_FieldNotSplit()
    {
        var storage = new MemoryStorage();
        await storage.SetAsync("data.csv", "id,text\n1,\"hello, world\"\n");

        var reader = new CsvFileReader(storage, idColumn: "id", textColumn: "text");

        var docs = await reader.ReadFilesAsync();

        docs.Should().HaveCount(1);
        docs[0].Text.Should().Be("hello, world");
    }

    [Fact]
    public async Task ReadFilesAsync_EscapedDoubleQuote_DecodedCorrectly()
    {
        var storage = new MemoryStorage();
        await storage.SetAsync("data.csv", "id,text\n1,\"say \"\"hello\"\"\"\n");

        var reader = new CsvFileReader(storage, idColumn: "id", textColumn: "text");

        var docs = await reader.ReadFilesAsync();

        docs.Should().HaveCount(1);
        docs[0].Text.Should().Be("say \"hello\"");
    }

    [Fact]
    public async Task ReadFilesAsync_EmptyContent_ReturnsEmpty()
    {
        var storage = new MemoryStorage();
        await storage.SetAsync("data.csv", string.Empty);

        var reader = new CsvFileReader(storage);

        var docs = await reader.ReadFilesAsync();

        docs.Should().BeEmpty();
    }

    [Fact]
    public async Task ReadFilesAsync_NoMatchingFiles_ReturnsEmpty()
    {
        var storage = new MemoryStorage();
        await storage.SetAsync("data.json", "{}");

        var reader = new CsvFileReader(storage);

        var docs = await reader.ReadFilesAsync();

        docs.Should().BeEmpty();
    }
}
