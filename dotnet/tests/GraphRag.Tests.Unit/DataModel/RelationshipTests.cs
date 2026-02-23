// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.DataModel;

namespace GraphRag.Tests.Unit.DataModel;

public class RelationshipTests
{
    [Fact]
    public void Relationship_DefaultWeight_IsOne()
    {
        var rel = new Relationship { Id = "r1", Source = "A", Target = "B" };

        rel.Weight.Should().Be(1.0);
    }

    [Fact]
    public void Relationship_Properties_SetCorrectly()
    {
        var embedding = new float[] { 0.3f, 0.4f };
        var textUnitIds = new[] { "tu1", "tu2" };
        var attributes = new Dictionary<string, object?> { ["strength"] = "high" };

        var rel = new Relationship
        {
            Id = "r1",
            ShortId = "sr1",
            Source = "EntityA",
            Target = "EntityB",
            Weight = 2.5,
            Description = "A strong relationship",
            DescriptionEmbedding = embedding,
            TextUnitIds = textUnitIds,
            Rank = 3,
            Attributes = attributes,
        };

        rel.Id.Should().Be("r1");
        rel.ShortId.Should().Be("sr1");
        rel.Source.Should().Be("EntityA");
        rel.Target.Should().Be("EntityB");
        rel.Weight.Should().Be(2.5);
        rel.Description.Should().Be("A strong relationship");
        rel.DescriptionEmbedding.Should().BeEquivalentTo(embedding);
        rel.TextUnitIds.Should().BeEquivalentTo(textUnitIds);
        rel.Rank.Should().Be(3);
        rel.Attributes.Should().ContainKey("strength");
    }
}
