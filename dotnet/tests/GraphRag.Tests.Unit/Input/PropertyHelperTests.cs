// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Input;

namespace GraphRag.Tests.Unit.Input;

/// <summary>
/// Unit tests for <see cref="PropertyHelper"/>.
/// </summary>
public class PropertyHelperTests
{
    [Fact]
    public void GetProperty_SimpleKey_ReturnsValue()
    {
        var data = new Dictionary<string, object?> { ["name"] = "Alice" };

        var result = PropertyHelper.GetProperty(data, "name");

        result.Should().Be("Alice");
    }

    [Fact]
    public void GetProperty_DotNotation_TraversesNested()
    {
        var nested = new Dictionary<string, object?> { ["city"] = "Seattle" };
        var data = new Dictionary<string, object?> { ["address"] = nested };

        var result = PropertyHelper.GetProperty(data, "address.city");

        result.Should().Be("Seattle");
    }

    [Fact]
    public void GenSha512Hash_SameInput_SameHash()
    {
        var item = new Dictionary<string, object?> { ["col1"] = "value1" };

        var hash1 = PropertyHelper.GenSha512Hash(item, "col1");
        var hash2 = PropertyHelper.GenSha512Hash(item, "col1");

        hash1.Should().Be(hash2);
        hash1.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void GenSha512Hash_DifferentInput_DifferentHash()
    {
        var item1 = new Dictionary<string, object?> { ["col"] = "alpha" };
        var item2 = new Dictionary<string, object?> { ["col"] = "beta" };

        var hash1 = PropertyHelper.GenSha512Hash(item1, "col");
        var hash2 = PropertyHelper.GenSha512Hash(item2, "col");

        hash1.Should().NotBe(hash2);
    }
}
