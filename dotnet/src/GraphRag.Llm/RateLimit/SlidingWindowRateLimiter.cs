// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Collections.Concurrent;

namespace GraphRag.Llm.RateLimit;

/// <summary>
/// A sliding window rate limiter that limits requests and tokens within a configurable time window.
/// </summary>
public sealed class SlidingWindowRateLimiter : IRateLimiter, IDisposable
{
    private readonly int _periodInSeconds;
    private readonly int? _requestsPerPeriod;
    private readonly int? _tokensPerPeriod;
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    private readonly ConcurrentQueue<(DateTimeOffset Timestamp, int Tokens)> _entries = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="SlidingWindowRateLimiter"/> class.
    /// </summary>
    /// <param name="periodInSeconds">The sliding window period in seconds.</param>
    /// <param name="requestsPerPeriod">The maximum number of requests allowed per period, or null for unlimited.</param>
    /// <param name="tokensPerPeriod">The maximum number of tokens allowed per period, or null for unlimited.</param>
    public SlidingWindowRateLimiter(int periodInSeconds = 60, int? requestsPerPeriod = null, int? tokensPerPeriod = null)
    {
        _periodInSeconds = periodInSeconds;
        _requestsPerPeriod = requestsPerPeriod;
        _tokensPerPeriod = tokensPerPeriod;
    }

    /// <inheritdoc />
    public async Task AcquireAsync(int tokenCount = 0, CancellationToken cancellationToken = default)
    {
        while (true)
        {
            cancellationToken.ThrowIfCancellationRequested();

            await _semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                CleanExpiredEntries();

                var currentRequests = _entries.Count;
                var currentTokens = 0;
                foreach (var entry in _entries)
                {
                    currentTokens += entry.Tokens;
                }

                var requestLimitReached = _requestsPerPeriod.HasValue && currentRequests >= _requestsPerPeriod.Value;
                var tokenLimitReached = _tokensPerPeriod.HasValue && currentTokens + tokenCount > _tokensPerPeriod.Value;

                if (!requestLimitReached && !tokenLimitReached)
                {
                    _entries.Enqueue((DateTimeOffset.UtcNow, tokenCount));
                    return;
                }
            }
            finally
            {
                _semaphore.Release();
            }

            // Wait a short interval before retrying
            await Task.Delay(100, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _semaphore.Dispose();
    }

    private void CleanExpiredEntries()
    {
        var cutoff = DateTimeOffset.UtcNow.AddSeconds(-_periodInSeconds);
        while (_entries.TryPeek(out var oldest) && oldest.Timestamp < cutoff)
        {
            _entries.TryDequeue(out _);
        }
    }
}
