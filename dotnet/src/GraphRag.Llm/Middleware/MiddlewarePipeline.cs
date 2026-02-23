// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Diagnostics;

using GraphRag.Cache;
using GraphRag.Llm.Types;

using Microsoft.Extensions.Logging;

namespace GraphRag.Llm.Middleware;

/// <summary>
/// Builds a middleware pipeline that chains logging, metrics, rate limiting, retries, cache, and the base completion function.
/// </summary>
public static partial class MiddlewarePipeline
{
    /// <summary>
    /// Builds a completion pipeline that wraps the base function with middleware layers.
    /// The order is: logging → metrics → rate limiting → retries → cache → base function.
    /// </summary>
    /// <param name="baseFunc">The base completion function to wrap.</param>
    /// <param name="logger">An optional logger for request logging.</param>
    /// <param name="metricsProcessor">An optional metrics processor.</param>
    /// <param name="rateLimiter">An optional rate limiter.</param>
    /// <param name="retry">An optional retry strategy.</param>
    /// <param name="cache">An optional cache.</param>
    /// <param name="cacheKeyCreator">An optional cache key creator function.</param>
    /// <returns>A function that processes completion requests through the middleware pipeline.</returns>
    public static Func<LlmCompletionArgs, CancellationToken, Task<LlmCompletionResponse>> BuildPipeline(
        Func<LlmCompletionArgs, CancellationToken, Task<LlmCompletionResponse>> baseFunc,
        ILogger? logger = null,
        IMetricsProcessor? metricsProcessor = null,
        IRateLimiter? rateLimiter = null,
        IRetry? retry = null,
        ICache? cache = null,
        Func<LlmCompletionArgs, string>? cacheKeyCreator = null)
    {
        var current = baseFunc;

        // Cache layer (innermost middleware)
        if (cache is not null && cacheKeyCreator is not null)
        {
            var inner = current;
            current = async (args, ct) =>
            {
                var key = cacheKeyCreator(args);
                var cached = await cache.GetAsync(key, ct).ConfigureAwait(false);
                if (cached is LlmCompletionResponse cachedResponse)
                {
                    return cachedResponse;
                }

                var result = await inner(args, ct).ConfigureAwait(false);
                await cache.SetAsync(key, result, cancellationToken: ct).ConfigureAwait(false);
                return result;
            };
        }

        // Retry layer
        if (retry is not null)
        {
            var inner = current;
            current = (args, ct) => retry.RetryAsync(() => inner(args, ct), ct);
        }

        // Rate limiting layer
        if (rateLimiter is not null)
        {
            var inner = current;
            current = async (args, ct) =>
            {
                await rateLimiter.AcquireAsync(cancellationToken: ct).ConfigureAwait(false);
                return await inner(args, ct).ConfigureAwait(false);
            };
        }

        // Metrics layer
        if (metricsProcessor is not null)
        {
            var inner = current;
            current = async (args, ct) =>
            {
                var sw = Stopwatch.StartNew();
                var result = await inner(args, ct).ConfigureAwait(false);
                sw.Stop();

                var metrics = MetricsHelper.Create(
                    ("duration_ms", sw.Elapsed.TotalMilliseconds),
                    ("prompt_tokens", result.Usage?.PromptTokens ?? 0),
                    ("completion_tokens", result.Usage?.CompletionTokens ?? 0),
                    ("total_tokens", result.Usage?.TotalTokens ?? 0));

                await metricsProcessor.ProcessAsync(metrics, ct).ConfigureAwait(false);
                return result;
            };
        }

        // Logging layer (outermost middleware)
        if (logger is not null)
        {
            var inner = current;
            current = async (args, ct) =>
            {
                LogCompletionRequest(logger, args.Messages.Count);
                var result = await inner(args, ct).ConfigureAwait(false);
                LogCompletionResponse(logger, result.Usage?.TotalTokens ?? 0);
                return result;
            };
        }

        return current;
    }

    [LoggerMessage(Level = LogLevel.Debug, Message = "LLM completion request: {MessageCount} messages")]
    private static partial void LogCompletionRequest(ILogger logger, int messageCount);

    [LoggerMessage(Level = LogLevel.Debug, Message = "LLM completion response: {TokenCount} tokens")]
    private static partial void LogCompletionResponse(ILogger logger, int tokenCount);
}
