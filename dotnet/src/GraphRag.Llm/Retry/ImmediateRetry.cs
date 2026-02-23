// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm.Retry;

/// <summary>
/// Retry strategy that retries immediately without any delay between attempts.
/// </summary>
public sealed class ImmediateRetry : IRetry
{
    private readonly int _maxRetries;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImmediateRetry"/> class.
    /// </summary>
    /// <param name="maxRetries">The maximum number of retry attempts.</param>
    public ImmediateRetry(int maxRetries = 3)
    {
        _maxRetries = maxRetries;
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
            }
        }

        throw new AggregateException($"Operation failed after {_maxRetries + 1} attempts.", lastException!);
    }
}
