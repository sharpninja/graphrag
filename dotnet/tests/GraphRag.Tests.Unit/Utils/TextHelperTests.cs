// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Utils;

namespace GraphRag.Tests.Unit.Utils;

/// <summary>
/// Unit tests for <see cref="TextHelper"/>.
/// </summary>
public class TextHelperTests
{
    [Fact]
    public void Truncate_ShortText_ReturnsSame()
    {
        var result = TextHelper.Truncate("hello", 10);
        result.Should().Be("hello");
    }

    [Fact]
    public void Truncate_LongText_TruncatesWithEllipsis()
    {
        var result = TextHelper.Truncate("hello world", 5);
        result.Should().Be("hello...");
    }
}
