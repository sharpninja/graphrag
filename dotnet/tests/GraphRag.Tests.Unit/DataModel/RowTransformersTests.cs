// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.DataModel;

namespace GraphRag.Tests.Unit.DataModel;

public class RowTransformersTests
{
    private static readonly string[] AbcList = ["a", "b", "c"];
    private static readonly string[] XyList = ["x", "y"];
    private static readonly string[] AbList = ["a", "b"];
    private static readonly string[] HelloWorldList = ["hello", "world"];
    private static readonly string[] Tu1Tu2List = ["tu1", "tu2"];
    private static readonly string[] Tu1List = ["tu1"];
    private static readonly string[] Tu1Tu2Tu3List = ["tu1", "tu2", "tu3"];

    [Theory]
    [InlineData(null, -1)]
    [InlineData("", -1)]
    [InlineData("42", 42)]
    [InlineData("notanumber", -1)]
    public void SafeInt_CoercesCorrectly(object? value, int expected)
    {
        RowTransformers.SafeInt(value).Should().Be(expected);
    }

    [Fact]
    public void SafeInt_IntValue_ReturnsDirectly()
    {
        RowTransformers.SafeInt(7).Should().Be(7);
    }

    [Fact]
    public void SafeInt_LongValue_CastsToInt()
    {
        RowTransformers.SafeInt(42L).Should().Be(42);
    }

    [Fact]
    public void SafeInt_CustomFill_ReturnsOnFailure()
    {
        RowTransformers.SafeInt(null, 99).Should().Be(99);
    }

    [Theory]
    [InlineData(null, 0.0)]
    [InlineData("", 0.0)]
    [InlineData("3.14", 3.14)]
    [InlineData("notanumber", 0.0)]
    public void SafeFloat_CoercesCorrectly(object? value, double expected)
    {
        RowTransformers.SafeFloat(value).Should().BeApproximately(expected, 0.001);
    }

    [Fact]
    public void SafeFloat_DoubleNaN_ReturnsFill()
    {
        RowTransformers.SafeFloat(double.NaN).Should().Be(0.0);
    }

    [Fact]
    public void SafeFloat_FloatNaN_ReturnsFill()
    {
        RowTransformers.SafeFloat(float.NaN).Should().Be(0.0);
    }

    [Fact]
    public void SafeFloat_DoubleValue_ReturnsDirect()
    {
        RowTransformers.SafeFloat(2.5).Should().Be(2.5);
    }

    [Fact]
    public void CoerceList_Null_ReturnsEmpty()
    {
        RowTransformers.CoerceList(null).Should().BeEmpty();
    }

    [Fact]
    public void CoerceList_StringList_ParsesCsv()
    {
        var result = RowTransformers.CoerceList("[a, b, c]");
        result.Should().BeEquivalentTo(AbcList);
    }

    [Fact]
    public void CoerceList_ExistingList_ReturnsSame()
    {
        var input = new List<string> { "x", "y" };
        var result = RowTransformers.CoerceList(input);
        result.Should().BeEquivalentTo(XyList);
    }

    [Fact]
    public void CoerceList_ReadOnlyList_CopiesList()
    {
        IReadOnlyList<string> input = new List<string> { "a", "b" };
        var result = RowTransformers.CoerceList(input);
        result.Should().BeEquivalentTo(AbList);
    }

    [Fact]
    public void CoerceList_QuotedCsv_StripsQuotes()
    {
        var result = RowTransformers.CoerceList("['hello', \"world\"]");
        result.Should().BeEquivalentTo(HelloWorldList);
    }

    [Fact]
    public void TransformEntityRow_CoercesTypes()
    {
        var row = new Dictionary<string, object?>
        {
            ["human_readable_id"] = "5",
            ["text_unit_ids"] = "[tu1, tu2]",
            ["frequency"] = "3",
            ["degree"] = "7",
        };

        var result = RowTransformers.TransformEntityRow(row);

        result["human_readable_id"].Should().Be(5);
        result["text_unit_ids"].Should().BeEquivalentTo(Tu1Tu2List);
        result["frequency"].Should().Be(3);
        result["degree"].Should().Be(7);
    }

    [Fact]
    public void TransformEntityRowForEmbedding_AddsTitleDescription()
    {
        var row = new Dictionary<string, object?>
        {
            ["title"] = "Entity1",
            ["description"] = "A test entity",
        };

        var result = RowTransformers.TransformEntityRowForEmbedding(row);

        result["title_description"].Should().Be("Entity1:A test entity");
    }

    [Fact]
    public void TransformRelationshipRow_CoercesTypes()
    {
        var row = new Dictionary<string, object?>
        {
            ["human_readable_id"] = "2",
            ["weight"] = "1.5",
            ["combined_degree"] = "10",
            ["text_unit_ids"] = "[tu1]",
        };

        var result = RowTransformers.TransformRelationshipRow(row);

        result["human_readable_id"].Should().Be(2);
        result["weight"].Should().Be(1.5);
        result["combined_degree"].Should().Be(10);
        result["text_unit_ids"].Should().BeEquivalentTo(Tu1List);
    }

    [Fact]
    public void TransformCommunityRow_CoercesTypes()
    {
        var row = new Dictionary<string, object?>
        {
            ["human_readable_id"] = "1",
            ["community"] = "3",
            ["level"] = "2",
            ["children"] = "[c1, c2]",
            ["entity_ids"] = "[e1]",
            ["relationship_ids"] = "[r1]",
            ["text_unit_ids"] = "[tu1]",
            ["period"] = "2025-01",
            ["size"] = "42",
        };

        var result = RowTransformers.TransformCommunityRow(row);

        result["human_readable_id"].Should().Be(1);
        result["community"].Should().Be(3);
        result["level"].Should().Be(2);
        result["size"].Should().Be(42);
        result["period"].Should().Be("2025-01");
    }

    [Fact]
    public void TransformCommunityReportRow_CoercesTypes()
    {
        var row = new Dictionary<string, object?>
        {
            ["human_readable_id"] = "0",
            ["community"] = "1",
            ["level"] = "0",
            ["children"] = string.Empty,
            ["rank"] = "4.5",
            ["findings"] = "[f1, f2]",
            ["size"] = "10",
        };

        var result = RowTransformers.TransformCommunityReportRow(row);

        result["rank"].Should().Be(4.5);
        result["size"].Should().Be(10);
    }

    [Fact]
    public void TransformTextUnitRow_CoercesTypes()
    {
        var row = new Dictionary<string, object?>
        {
            ["human_readable_id"] = "3",
            ["n_tokens"] = "100",
            ["entity_ids"] = "[e1, e2]",
            ["relationship_ids"] = "[r1]",
            ["covariate_ids"] = "[cv1]",
        };

        var result = RowTransformers.TransformTextUnitRow(row);

        result["human_readable_id"].Should().Be(3);
        result["n_tokens"].Should().Be(100);
    }

    [Fact]
    public void TransformDocumentRow_CoercesTypes()
    {
        var row = new Dictionary<string, object?>
        {
            ["human_readable_id"] = "1",
            ["text_unit_ids"] = "[tu1, tu2, tu3]",
        };

        var result = RowTransformers.TransformDocumentRow(row);

        result["human_readable_id"].Should().Be(1);
        result["text_unit_ids"].Should().BeEquivalentTo(Tu1Tu2Tu3List);
    }

    [Fact]
    public void TransformCovariateRow_CoercesTypes()
    {
        var row = new Dictionary<string, object?>
        {
            ["human_readable_id"] = "7",
        };

        var result = RowTransformers.TransformCovariateRow(row);

        result["human_readable_id"].Should().Be(7);
    }

    [Fact]
    public void CoerceList_EmptyBrackets_ReturnsEmpty()
    {
        RowTransformers.CoerceList("[]").Should().BeEmpty();
    }

    [Fact]
    public void CoerceList_NoBrackets_ParsesAsCsv()
    {
        var result = RowTransformers.CoerceList("a, b, c");
        result.Should().BeEquivalentTo(AbcList);
    }
}
