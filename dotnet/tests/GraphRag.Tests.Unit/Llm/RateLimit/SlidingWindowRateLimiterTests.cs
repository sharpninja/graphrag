// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Llm.RateLimit;

namespace GraphRag.Tests.Unit.Llm.RateLimit;

/// <summary>
/// Unit tests for <see cref="SlidingWindowRateLimiter"/>.
/// </summary>
public class SlidingWindowRateLimiterTests
{
    [Fact]
    public async Task AcquireAsync_WithinLimit_DoesNotBlock()
    {
        using var sut = new SlidingWindowRateLimiter(periodInSeconds: 60, requestsPerPeriod: 5);

        var sw = System.Diagnostics.Stopwatch.StartNew();
        await sut.AcquireAsync();
        sw.Stop();

        sw.ElapsedMilliseconds.Should().BeLessThan(1000);
    }

    [Fact]
    public async Task AcquireAsync_ExceedsLimit_Waits()
    {
        using var sut = new SlidingWindowRateLimiter(periodInSeconds: 1, requestsPerPeriod: 1);

        await sut.AcquireAsync();

        var sw = System.Diagnostics.Stopwatch.StartNew();
        await sut.AcquireAsync();
        sw.Stop();

        sw.ElapsedMilliseconds.Should().BeGreaterThanOrEqualTo(100);
    }
}
