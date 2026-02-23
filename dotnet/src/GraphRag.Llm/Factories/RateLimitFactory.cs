// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Common.Factories;
using GraphRag.Llm.Config;
using GraphRag.Llm.RateLimit;

namespace GraphRag.Llm.Factories;

/// <summary>
/// Factory for creating <see cref="IRateLimiter"/> instances by strategy name.
/// </summary>
public sealed class RateLimitFactory : ServiceFactory<IRateLimiter>
{
    private bool _builtinsRegistered;

    /// <summary>
    /// Registers the built-in rate limiters if not already registered.
    /// </summary>
    public void EnsureBuiltins()
    {
        if (_builtinsRegistered)
        {
            return;
        }

        Register(
            RateLimitType.SlidingWindow,
            args =>
            {
                var period = args.TryGetValue("periodInSeconds", out var p) && p is int ps ? ps : 60;
                var requests = args.TryGetValue("requestsPerPeriod", out var r) && r is int rp ? (int?)rp : null;
                var tokens = args.TryGetValue("tokensPerPeriod", out var t) && t is int tp ? (int?)tp : null;
                return new SlidingWindowRateLimiter(period, requests, tokens);
            },
            ServiceScope.Singleton);

        _builtinsRegistered = true;
    }
}
