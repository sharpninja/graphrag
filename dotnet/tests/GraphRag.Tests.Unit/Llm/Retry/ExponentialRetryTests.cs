// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Llm.Retry;

namespace GraphRag.Tests.Unit.Llm.Retry;

/// <summary>
/// Unit tests for <see cref="ExponentialRetry"/>.
/// </summary>
public class ExponentialRetryTests
{
    [Fact]
    public async Task RetryAsync_SucceedsOnFirstTry_ReturnsResult()
    {
        var sut = new ExponentialRetry(maxRetries: 3, baseDelay: 0.01, jitter: false);

        var result = await sut.RetryAsync(() => Task.FromResult(42));

        result.Should().Be(42);
    }

    [Fact]
    public async Task RetryAsync_FailsThenSucceeds_RetriesAndReturns()
    {
        var sut = new ExponentialRetry(maxRetries: 3, baseDelay: 0.01, jitter: false);
        var attempt = 0;

        var result = await sut.RetryAsync<int>(() =>
        {
            attempt++;
            if (attempt < 3)
            {
                throw new InvalidOperationException("fail");
            }

            return Task.FromResult(99);
        });

        result.Should().Be(99);
        attempt.Should().Be(3);
    }

    [Fact]
    public async Task RetryAsync_ExceedsMaxRetries_ThrowsLastException()
    {
        var sut = new ExponentialRetry(maxRetries: 2, baseDelay: 0.01, jitter: false);

        var act = async () => await sut.RetryAsync<int>(() =>
            throw new InvalidOperationException("always fails"));

        await act.Should().ThrowAsync<AggregateException>()
            .WithMessage("*3 attempts*");
    }

    [Fact]
    public void Retry_Sync_Works()
    {
        var sut = new ExponentialRetry(maxRetries: 2, baseDelay: 0.01, jitter: false);
        var attempt = 0;

        var result = sut.Retry(() =>
        {
            attempt++;
            if (attempt < 2)
            {
                throw new InvalidOperationException("fail");
            }

            return "ok";
        });

        result.Should().Be("ok");
        attempt.Should().Be(2);
    }
}
