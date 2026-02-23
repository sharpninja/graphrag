// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm;

/// <summary>
/// Interface for rate limiting operations.
/// </summary>
public interface IRateLimiter
{
    /// <summary>
    /// Acquires permission to proceed, waiting if necessary to comply with rate limits.
    /// </summary>
    /// <param name="tokenCount">The number of tokens to account for in the rate limit.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A task that completes when permission is acquired.</returns>
    Task AcquireAsync(int tokenCount = 0, CancellationToken cancellationToken = default);
}
