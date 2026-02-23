// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.DataModel;

namespace GraphRag.Tests.Unit.DataModel;

public class EntityTests
{
    [Fact]
    public void Entity_DefaultRank_IsOne()
    {
        var entity = new Entity { Id = "1", Title = "Test" };

        entity.Rank.Should().Be(1);
    }

    [Fact]
    public void Entity_WithAllProperties_SetsCorrectly()
    {
        var embedding = new float[] { 0.1f, 0.2f };
        var communityIds = new[] { "c1", "c2" };
        var textUnitIds = new[] { "tu1" };
        var attributes = new Dictionary<string, object?> { ["key"] = "value" };

        var entity = new Entity
        {
            Id = "e1",
            ShortId = "short1",
            Title = "Entity1",
            Type = "person",
            Description = "A test entity",
            DescriptionEmbedding = embedding,
            NameEmbedding = embedding,
            CommunityIds = communityIds,
            TextUnitIds = textUnitIds,
            Rank = 5,
            Attributes = attributes,
        };

        entity.Id.Should().Be("e1");
        entity.ShortId.Should().Be("short1");
        entity.Title.Should().Be("Entity1");
        entity.Type.Should().Be("person");
        entity.Description.Should().Be("A test entity");
        entity.DescriptionEmbedding.Should().BeEquivalentTo(embedding);
        entity.NameEmbedding.Should().BeEquivalentTo(embedding);
        entity.CommunityIds.Should().BeEquivalentTo(communityIds);
        entity.TextUnitIds.Should().BeEquivalentTo(textUnitIds);
        entity.Rank.Should().Be(5);
        entity.Attributes.Should().ContainKey("key");
    }

    [Fact]
    public void Entity_RecordEquality_WorksCorrectly()
    {
        var entity1 = new Entity { Id = "e1", Title = "Entity1", Type = "person", Rank = 1 };
        var entity2 = new Entity { Id = "e1", Title = "Entity1", Type = "person", Rank = 1 };
        var entity3 = new Entity { Id = "e2", Title = "Entity2", Type = "org", Rank = 2 };

        entity1.Should().Be(entity2);
        entity1.Should().NotBe(entity3);
    }
}
