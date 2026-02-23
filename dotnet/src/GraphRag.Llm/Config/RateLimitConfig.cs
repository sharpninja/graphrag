// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm.Config;

/// <summary>
/// Configuration for rate limiting.
/// </summary>
public sealed record RateLimitConfig
{
    /// <summary>Gets the rate limiting strategy type.</summary>
    public string Type { get; init; } = RateLimitType.SlidingWindow;

    /// <summary>Gets the maximum number of requests per window.</summary>
    public int? RequestsPerMinute { get; init; }

    /// <summary>Gets the maximum number of tokens per window.</summary>
    public int? TokensPerMinute { get; init; }

    /// <summary>Gets the window size in seconds.</summary>
    public int? WindowSeconds { get; init; }
}
