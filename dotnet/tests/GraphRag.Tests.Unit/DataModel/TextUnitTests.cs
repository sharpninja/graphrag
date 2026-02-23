// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.DataModel;

namespace GraphRag.Tests.Unit.DataModel;

public class TextUnitTests
{
    [Fact]
    public void TextUnit_Properties_SetCorrectly()
    {
        var entityIds = new[] { "e1", "e2" };
        var relationshipIds = new[] { "r1" };
        var attributes = new Dictionary<string, object?> { ["source"] = "doc1" };

        var textUnit = new TextUnit
        {
            Id = "tu1",
            ShortId = "s1",
            Text = "Some text content",
            EntityIds = entityIds,
            RelationshipIds = relationshipIds,
            NTokens = 100,
            DocumentId = "doc1",
            Attributes = attributes,
        };

        textUnit.Id.Should().Be("tu1");
        textUnit.ShortId.Should().Be("s1");
        textUnit.Text.Should().Be("Some text content");
        textUnit.EntityIds.Should().BeEquivalentTo(entityIds);
        textUnit.RelationshipIds.Should().BeEquivalentTo(relationshipIds);
        textUnit.NTokens.Should().Be(100);
        textUnit.DocumentId.Should().Be("doc1");
        textUnit.Attributes.Should().ContainKey("source");
    }
}
