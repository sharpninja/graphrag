// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm.Retry;

/// <summary>
/// Retry strategy that uses exponential backoff with optional jitter.
/// </summary>
public sealed class ExponentialRetry : IRetry
{
    private readonly int _maxRetries;
    private readonly double _baseDelay;
    private readonly bool _jitter;
    private readonly double? _maxDelay;

    /// <summary>
    /// Initializes a new instance of the <see cref="ExponentialRetry"/> class.
    /// </summary>
    /// <param name="maxRetries">The maximum number of retry attempts.</param>
    /// <param name="baseDelay">The base delay in seconds between retries.</param>
    /// <param name="jitter">Whether to add random jitter to the delay.</param>
    /// <param name="maxDelay">The maximum delay in seconds, or null for no cap.</param>
    public ExponentialRetry(int maxRetries = 7, double baseDelay = 2.0, bool jitter = true, double? maxDelay = null)
    {
        _maxRetries = maxRetries;
        _baseDelay = baseDelay;
        _jitter = jitter;
        _maxDelay = maxDelay;
    }

    /// <inheritdoc />
    public async Task<T> RetryAsync<T>(Func<Task<T>> func, CancellationToken cancellationToken = default)
    {
        Exception? lastException = null;

        for (var attempt = 0; attempt <= _maxRetries; attempt++)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                return await func().ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                lastException = ex;

                if (attempt == _maxRetries)
                {
                    break;
                }

                var delay = _baseDelay * Math.Pow(2, attempt);
                if (_maxDelay.HasValue)
                {
                    delay = Math.Min(delay, _maxDelay.Value);
                }

                if (_jitter)
                {
                    delay *= 0.5 + Random.Shared.NextDouble();
                }

                await Task.Delay(TimeSpan.FromSeconds(delay), cancellationToken).ConfigureAwait(false);
            }
        }

        throw new AggregateException($"Operation failed after {_maxRetries + 1} attempts.", lastException!);
    }

    /// <inheritdoc />
    public T Retry<T>(Func<T> func)
    {
        Exception? lastException = null;

        for (var attempt = 0; attempt <= _maxRetries; attempt++)
        {
            try
            {
                return func();
            }
            catch (Exception ex)
            {
                lastException = ex;

                if (attempt == _maxRetries)
                {
                    break;
                }

                var delay = _baseDelay * Math.Pow(2, attempt);
                if (_maxDelay.HasValue)
                {
                    delay = Math.Min(delay, _maxDelay.Value);
                }

                if (_jitter)
                {
                    delay *= 0.5 + Random.Shared.NextDouble();
                }

                Thread.Sleep(TimeSpan.FromSeconds(delay));
            }
        }

        throw new AggregateException($"Operation failed after {_maxRetries + 1} attempts.", lastException!);
    }
}
