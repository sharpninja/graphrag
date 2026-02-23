// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Vectors;

namespace GraphRag.Tests.Unit.Vectors;

/// <summary>
/// Unit tests for <see cref="TimestampHelper"/>.
/// </summary>
public class TimestampHelperTests
{
    [Fact]
    public void ExplodeTimestamp_ValidIso_ReturnsFields()
    {
        var result = TimestampHelper.ExplodeTimestamp("2024-06-15T10:30:45Z");

        result["create_date_year"].Should().Be(2024);
        result["create_date_month"].Should().Be(6);
        result["create_date_day"].Should().Be(15);
        result["create_date_hour"].Should().Be(10);
        result["create_date_minute"].Should().Be(30);
        result["create_date_second"].Should().Be(45);
    }

    [Fact]
    public void ExplodeTimestamp_Null_ReturnsEmptyDict()
    {
        var result = TimestampHelper.ExplodeTimestamp(null);
        result.Should().BeEmpty();
    }
}
