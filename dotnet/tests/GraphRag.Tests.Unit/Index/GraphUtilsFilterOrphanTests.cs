// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.DataModel;
using GraphRag.Index.Operations;

namespace GraphRag.Tests.Unit.Index;

public class GraphUtilsFilterOrphanTests
{
    [Fact]
    public void FilterOrphanRelationships_EmptyRelationships_ReturnsEmpty()
    {
        var entities = new List<Entity>
        {
            new() { Id = "1", Title = "A" },
        };

        var result = GraphUtils.FilterOrphanRelationships([], entities);

        result.Should().BeEmpty();
    }

    [Fact]
    public void FilterOrphanRelationships_EmptyEntities_ReturnsEmpty()
    {
        var rels = new List<Relationship>
        {
            new() { Id = "r1", Source = "A", Target = "B" },
        };

        var result = GraphUtils.FilterOrphanRelationships(rels, []);

        result.Should().BeEmpty();
    }

    [Fact]
    public void FilterOrphanRelationships_AllValid_ReturnsAll()
    {
        var entities = new List<Entity>
        {
            new() { Id = "1", Title = "A" },
            new() { Id = "2", Title = "B" },
            new() { Id = "3", Title = "C" },
        };

        var rels = new List<Relationship>
        {
            new() { Id = "r1", Source = "A", Target = "B" },
            new() { Id = "r2", Source = "B", Target = "C" },
        };

        var result = GraphUtils.FilterOrphanRelationships(rels, entities);

        result.Should().HaveCount(2);
    }

    [Fact]
    public void FilterOrphanRelationships_OrphanSource_DropsRelationship()
    {
        var entities = new List<Entity>
        {
            new() { Id = "1", Title = "A" },
            new() { Id = "2", Title = "B" },
        };

        var rels = new List<Relationship>
        {
            new() { Id = "r1", Source = "A", Target = "B" },
            new() { Id = "r2", Source = "PHANTOM", Target = "B" },
        };

        var result = GraphUtils.FilterOrphanRelationships(rels, entities);

        result.Should().HaveCount(1);
        result[0].Id.Should().Be("r1");
    }

    [Fact]
    public void FilterOrphanRelationships_OrphanTarget_DropsRelationship()
    {
        var entities = new List<Entity>
        {
            new() { Id = "1", Title = "A" },
            new() { Id = "2", Title = "B" },
        };

        var rels = new List<Relationship>
        {
            new() { Id = "r1", Source = "A", Target = "B" },
            new() { Id = "r2", Source = "A", Target = "PHANTOM" },
        };

        var result = GraphUtils.FilterOrphanRelationships(rels, entities);

        result.Should().HaveCount(1);
        result[0].Id.Should().Be("r1");
    }

    [Fact]
    public void FilterOrphanRelationships_BothOrphan_DropsAll()
    {
        var entities = new List<Entity>
        {
            new() { Id = "1", Title = "A" },
        };

        var rels = new List<Relationship>
        {
            new() { Id = "r1", Source = "X", Target = "Y" },
        };

        var result = GraphUtils.FilterOrphanRelationships(rels, entities);

        result.Should().BeEmpty();
    }

    [Fact]
    public void FilterOrphanRelationships_CaseSensitive_MatchesExactly()
    {
        var entities = new List<Entity>
        {
            new() { Id = "1", Title = "Alice" },
            new() { Id = "2", Title = "Bob" },
        };

        var rels = new List<Relationship>
        {
            new() { Id = "r1", Source = "Alice", Target = "Bob" },
            new() { Id = "r2", Source = "alice", Target = "bob" },
        };

        var result = GraphUtils.FilterOrphanRelationships(rels, entities);

        result.Should().HaveCount(1);
        result[0].Id.Should().Be("r1");
    }
}
