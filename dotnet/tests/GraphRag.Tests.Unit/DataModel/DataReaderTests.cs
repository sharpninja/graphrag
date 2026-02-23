// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.DataModel;
using GraphRag.Storage.Tables;
using Moq;

namespace GraphRag.Tests.Unit.DataModel;

public class DataReaderTests
{
    private readonly Mock<ITableProvider> _mockProvider = new();

    [Fact]
    public async Task EntitiesAsync_ReturnsTypedEntities()
    {
        var rows = new List<Dictionary<string, object?>>
        {
            new()
            {
                ["id"] = "e1",
                ["title"] = "Entity One",
                ["type"] = "person",
                ["rank"] = 3,
            },
            new()
            {
                ["id"] = "e2",
                ["title"] = "Entity Two",
                ["type"] = "org",
            },
        };
        _mockProvider.Setup(p => p.ReadAsync("create_final_entities", It.IsAny<CancellationToken>()))
            .ReturnsAsync(rows);

        var reader = new DataReader(_mockProvider.Object);
        var entities = await reader.EntitiesAsync();

        entities.Should().HaveCount(2);
        entities[0].Id.Should().Be("e1");
        entities[0].Title.Should().Be("Entity One");
        entities[0].Type.Should().Be("person");
        entities[0].Rank.Should().Be(3);
        entities[1].Id.Should().Be("e2");
    }

    [Fact]
    public async Task RelationshipsAsync_ReturnsTypedRelationships()
    {
        var rows = new List<Dictionary<string, object?>>
        {
            new()
            {
                ["id"] = "r1",
                ["source"] = "e1",
                ["target"] = "e2",
                ["weight"] = 2.5,
                ["description"] = "related",
            },
        };
        _mockProvider.Setup(p => p.ReadAsync("create_final_relationships", It.IsAny<CancellationToken>()))
            .ReturnsAsync(rows);

        var reader = new DataReader(_mockProvider.Object);
        var rels = await reader.RelationshipsAsync();

        rels.Should().HaveCount(1);
        rels[0].Source.Should().Be("e1");
        rels[0].Target.Should().Be("e2");
    }

    [Fact]
    public async Task CommunitiesAsync_ReturnsTypedCommunities()
    {
        var rows = new List<Dictionary<string, object?>>
        {
            new()
            {
                ["id"] = "c1",
                ["title"] = "Community One",
                ["level"] = "0",
                ["parent"] = string.Empty,
                ["children"] = (IReadOnlyList<string>)["c2", "c3"],
                ["size"] = 10,
            },
        };
        _mockProvider.Setup(p => p.ReadAsync("create_final_communities", It.IsAny<CancellationToken>()))
            .ReturnsAsync(rows);

        var reader = new DataReader(_mockProvider.Object);
        var communities = await reader.CommunitiesAsync();

        communities.Should().HaveCount(1);
        communities[0].Level.Should().Be("0");
        communities[0].Size.Should().Be(10);
    }

    [Fact]
    public async Task CommunityReportsAsync_ReturnsTypedReports()
    {
        var rows = new List<Dictionary<string, object?>>
        {
            new()
            {
                ["id"] = "cr1",
                ["title"] = "Report One",
                ["community_id"] = "c1",
                ["summary"] = "A summary",
                ["full_content"] = "Full content here",
                ["rank"] = 2.5,
            },
        };
        _mockProvider.Setup(p => p.ReadAsync("create_final_community_reports", It.IsAny<CancellationToken>()))
            .ReturnsAsync(rows);

        var reader = new DataReader(_mockProvider.Object);
        var reports = await reader.CommunityReportsAsync();

        reports.Should().HaveCount(1);
        reports[0].CommunityId.Should().Be("c1");
        reports[0].Rank.Should().Be(2.5);
    }

    [Fact]
    public async Task CovariatesAsync_ReturnsTypedCovariates()
    {
        var rows = new List<Dictionary<string, object?>>
        {
            new()
            {
                ["id"] = "cv1",
                ["subject_id"] = "e1",
                ["subject_type"] = "entity",
                ["covariate_type"] = "claim",
            },
        };
        _mockProvider.Setup(p => p.ReadAsync("create_final_covariates", It.IsAny<CancellationToken>()))
            .ReturnsAsync(rows);

        var reader = new DataReader(_mockProvider.Object);
        var covariates = await reader.CovariatesAsync();

        covariates.Should().HaveCount(1);
        covariates[0].SubjectId.Should().Be("e1");
    }

    [Fact]
    public async Task TextUnitsAsync_ReturnsTypedTextUnits()
    {
        var rows = new List<Dictionary<string, object?>>
        {
            new()
            {
                ["id"] = "tu1",
                ["text"] = "Some text content",
                ["n_tokens"] = 42,
                ["document_id"] = "d1",
            },
        };
        _mockProvider.Setup(p => p.ReadAsync("create_final_text_units", It.IsAny<CancellationToken>()))
            .ReturnsAsync(rows);

        var reader = new DataReader(_mockProvider.Object);
        var units = await reader.TextUnitsAsync();

        units.Should().HaveCount(1);
        units[0].Text.Should().Be("Some text content");
        units[0].NTokens.Should().Be(42);
        units[0].DocumentId.Should().Be("d1");
    }

    [Fact]
    public async Task DocumentsAsync_ReturnsTypedDocuments()
    {
        var rows = new List<Dictionary<string, object?>>
        {
            new()
            {
                ["id"] = "d1",
                ["title"] = "Document One",
                ["type"] = "text",
                ["text"] = "Document content",
            },
        };
        _mockProvider.Setup(p => p.ReadAsync("create_final_documents", It.IsAny<CancellationToken>()))
            .ReturnsAsync(rows);

        var reader = new DataReader(_mockProvider.Object);
        var docs = await reader.DocumentsAsync();

        docs.Should().HaveCount(1);
        docs[0].Title.Should().Be("Document One");
        docs[0].Text.Should().Be("Document content");
    }

    [Fact]
    public void Constructor_ThrowsOnNull()
    {
        var act = () => new DataReader(null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public async Task EmptyTable_ReturnsEmptyList()
    {
        _mockProvider.Setup(p => p.ReadAsync("create_final_entities", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Dictionary<string, object?>>());

        var reader = new DataReader(_mockProvider.Object);
        var entities = await reader.EntitiesAsync();

        entities.Should().BeEmpty();
    }
}
