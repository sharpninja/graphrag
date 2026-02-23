// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Vectors;

namespace GraphRag.Tests.Unit.Vectors;

/// <summary>
/// Unit tests for <see cref="IndexSchema"/>.
/// </summary>
public class IndexSchemaTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        var schema = new IndexSchema { IndexName = "test" };

        schema.IdField.Should().Be("id");
        schema.VectorField.Should().Be("vector");
        schema.VectorSize.Should().Be(3072);
        schema.Fields.Should().BeNull();
    }
}
