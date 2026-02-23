// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Llm.Retry;

namespace GraphRag.Tests.Unit.Llm.Retry;

/// <summary>
/// Unit tests for <see cref="ImmediateRetry"/>.
/// </summary>
public class ImmediateRetryTests
{
    [Fact]
    public async Task RetryAsync_FailsThenSucceeds_RetriesImmediately()
    {
        var sut = new ImmediateRetry(maxRetries: 3);
        var attempt = 0;

        var result = await sut.RetryAsync<int>(() =>
        {
            attempt++;
            if (attempt < 3)
            {
                throw new InvalidOperationException("fail");
            }

            return Task.FromResult(42);
        });

        result.Should().Be(42);
        attempt.Should().Be(3);
    }

    [Fact]
    public async Task RetryAsync_ExceedsMaxRetries_Throws()
    {
        var sut = new ImmediateRetry(maxRetries: 1);

        var act = async () => await sut.RetryAsync<int>(() =>
            throw new InvalidOperationException("always fails"));

        await act.Should().ThrowAsync<AggregateException>()
            .WithMessage("*2 attempts*");
    }
}
