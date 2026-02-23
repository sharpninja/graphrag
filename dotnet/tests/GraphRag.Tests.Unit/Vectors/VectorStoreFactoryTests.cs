// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Vectors;

namespace GraphRag.Tests.Unit.Vectors;

/// <summary>
/// Unit tests for <see cref="VectorStoreFactory"/>.
/// </summary>
public class VectorStoreFactoryTests
{
    private readonly VectorStoreFactory _factory = new();

    [Fact]
    public void CreateVectorStore_UnknownType_Throws()
    {
        var config = new VectorStoreConfig { Type = "unknown" };
        var schema = new IndexSchema { IndexName = "test" };

        var act = () => _factory.CreateVectorStore(config, schema);
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void CreateVectorStore_LanceDb_ThrowsNotImplemented()
    {
        var config = new VectorStoreConfig { Type = VectorStoreType.LanceDb };
        var schema = new IndexSchema { IndexName = "test" };

        var act = () => _factory.CreateVectorStore(config, schema);
        act.Should().Throw<NotImplementedException>();
    }
}
