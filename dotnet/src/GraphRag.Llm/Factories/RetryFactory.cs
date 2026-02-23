// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Common.Factories;
using GraphRag.Llm.Config;
using GraphRag.Llm.Retry;

namespace GraphRag.Llm.Factories;

/// <summary>
/// Factory for creating <see cref="IRetry"/> instances by strategy name.
/// </summary>
public sealed class RetryFactory : ServiceFactory<IRetry>
{
    private bool _builtinsRegistered;

    /// <summary>
    /// Registers the built-in retry strategies if not already registered.
    /// </summary>
    public void EnsureBuiltins()
    {
        if (_builtinsRegistered)
        {
            return;
        }

        Register(
            RetryType.ExponentialBackoff,
            args =>
            {
                var maxRetries = args.TryGetValue("maxRetries", out var mr) && mr is int m ? m : 7;
                var baseDelay = args.TryGetValue("baseDelay", out var bd) && bd is double b ? b : 2.0;
                var jitter = !args.TryGetValue("jitter", out var j) || j is not bool jb || jb;
                var maxDelay = args.TryGetValue("maxDelay", out var md) && md is double d ? (double?)d : null;
                return new ExponentialRetry(maxRetries, baseDelay, jitter, maxDelay);
            },
            ServiceScope.Singleton);

        Register(
            RetryType.Immediate,
            args =>
            {
                var maxRetries = args.TryGetValue("maxRetries", out var mr) && mr is int m ? m : 3;
                return new ImmediateRetry(maxRetries);
            },
            ServiceScope.Singleton);

        _builtinsRegistered = true;
    }
}
