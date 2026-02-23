// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Logger;

namespace GraphRag.Tests.Unit.Logger;

/// <summary>
/// Unit tests for <see cref="ProgressTicker"/>.
/// </summary>
public class ProgressTickerTests
{
    [Fact]
    public void Tick_IncrementsCompletedItems()
    {
        ProgressInfo? captured = null;
        var ticker = new ProgressTicker(p => captured = p, totalItems: 10);

        ticker.Tick();
        ticker.CompletedItems.Should().Be(1);
        captured.Should().NotBeNull();
        captured!.CompletedItems.Should().Be(1);
        captured.TotalItems.Should().Be(10);

        ticker.Tick(3);
        ticker.CompletedItems.Should().Be(4);
        captured!.CompletedItems.Should().Be(4);
    }

    [Fact]
    public void Done_SetsCompletedToTotal()
    {
        ProgressInfo? captured = null;
        var ticker = new ProgressTicker(p => captured = p, totalItems: 5);

        ticker.Tick(5);
        ticker.Done();

        captured.Should().NotBeNull();
        captured!.CompletedItems.Should().Be(5);
        captured.TotalItems.Should().Be(5);
        captured.Description.Should().Be("Done");
    }
}
