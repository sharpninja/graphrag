// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.DataModel;

namespace GraphRag.Tests.Unit.DataModel;

public class CommunityTests
{
    [Fact]
    public void Community_Properties_SetCorrectly()
    {
        var children = new[] { "child1", "child2" };
        var entityIds = new[] { "e1", "e2" };
        var relationshipIds = new[] { "r1" };
        var textUnitIds = new[] { "tu1" };
        var attributes = new Dictionary<string, object?> { ["category"] = "science" };

        var community = new Community
        {
            Id = "c1",
            Title = "Community1",
            Level = "0",
            Parent = "root",
            Children = children,
            EntityIds = entityIds,
            RelationshipIds = relationshipIds,
            TextUnitIds = textUnitIds,
            Attributes = attributes,
            Size = 42,
            Period = "2024",
        };

        community.Id.Should().Be("c1");
        community.Title.Should().Be("Community1");
        community.Level.Should().Be("0");
        community.Parent.Should().Be("root");
        community.Children.Should().BeEquivalentTo(children);
        community.EntityIds.Should().BeEquivalentTo(entityIds);
        community.RelationshipIds.Should().BeEquivalentTo(relationshipIds);
        community.TextUnitIds.Should().BeEquivalentTo(textUnitIds);
        community.Attributes.Should().ContainKey("category");
        community.Size.Should().Be(42);
        community.Period.Should().Be("2024");
    }
}
