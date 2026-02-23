// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.DataModel;

namespace GraphRag.Tests.Unit.DataModel;

public class DocumentTests
{
    [Fact]
    public void Document_DefaultType_IsText()
    {
        var doc = new Document { Id = "d1", Title = "Doc1" };

        doc.Type.Should().Be("text");
    }

    [Fact]
    public void Document_DefaultText_IsEmpty()
    {
        var doc = new Document { Id = "d1", Title = "Doc1" };

        doc.Text.Should().BeEmpty();
        doc.TextUnitIds.Should().BeEmpty();
    }
}
