// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Vectors;

namespace GraphRag.Tests.Unit.Vectors;

/// <summary>
/// Unit tests for <see cref="VectorStoreDocument"/>.
/// </summary>
public class VectorStoreDocumentTests
{
    [Fact]
    public void Record_Properties_SetCorrectly()
    {
        var vector = new float[] { 0.1f, 0.2f, 0.3f };
        var data = new Dictionary<string, object?> { ["key"] = "value" };

        var doc = new VectorStoreDocument
        {
            Id = "doc-1",
            Vector = vector,
            Data = data,
            CreateDate = "2024-01-01T00:00:00Z",
            UpdateDate = "2024-06-15T12:00:00Z",
        };

        doc.Id.Should().Be("doc-1");
        doc.Vector.Should().BeEquivalentTo(vector);
        doc.Data.Should().ContainKey("key");
        doc.CreateDate.Should().Be("2024-01-01T00:00:00Z");
        doc.UpdateDate.Should().Be("2024-06-15T12:00:00Z");
    }
}
