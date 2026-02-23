// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm.Config;

/// <summary>
/// Known rate limiting strategies.
/// </summary>
public static class RateLimitType
{
    /// <summary>Sliding window rate limiting.</summary>
    public const string SlidingWindow = "sliding_window";
}
