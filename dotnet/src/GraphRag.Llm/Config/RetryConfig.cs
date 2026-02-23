// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm.Config;

/// <summary>
/// Configuration for retry behavior.
/// </summary>
public sealed record RetryConfig
{
    /// <summary>Gets the retry strategy type.</summary>
    public string Type { get; init; } = RetryType.ExponentialBackoff;

    /// <summary>Gets the maximum number of retry attempts.</summary>
    public int MaxRetries { get; init; } = 3;

    /// <summary>Gets the initial delay in seconds before the first retry.</summary>
    public double InitialDelay { get; init; } = 1.0;

    /// <summary>Gets the maximum delay in seconds between retries.</summary>
    public double MaxDelay { get; init; } = 60.0;

    /// <summary>Gets the backoff multiplier for exponential backoff.</summary>
    public double BackoffMultiplier { get; init; } = 2.0;
}
