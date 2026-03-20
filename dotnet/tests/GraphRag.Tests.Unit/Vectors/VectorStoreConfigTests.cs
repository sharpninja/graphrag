// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Vectors;

namespace GraphRag.Tests.Unit.Vectors;

/// <summary>
/// Unit tests for <see cref="VectorStoreConfig"/>.
/// </summary>
public class VectorStoreConfigTests
{
    [Fact]
    public void WithVectorSize_ReturnsUpdatedCopy_AndSynchronizesSchema()
    {
        var config = new VectorStoreConfig
        {
            Type = "azure_ai_search",
            VectorSize = 3072,
            IndexSchema = new IndexSchema { IndexName = "entities", VectorSize = 3072 },
        };

        var updated = config.WithVectorSize(1536);

        updated.Should().NotBeSameAs(config);
        updated.VectorSize.Should().Be(1536);
        updated.IndexSchema.Should().NotBeNull();
        updated.IndexSchema!.VectorSize.Should().Be(1536);
        config.VectorSize.Should().Be(3072);
        config.IndexSchema!.VectorSize.Should().Be(3072);
    }
}
