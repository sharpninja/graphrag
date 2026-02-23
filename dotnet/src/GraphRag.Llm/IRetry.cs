// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm;

/// <summary>
/// Interface for retrying operations with configurable strategies.
/// </summary>
public interface IRetry
{
    /// <summary>
    /// Retries the specified asynchronous function according to the configured strategy.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="func">The asynchronous function to retry.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The result of the function.</returns>
    Task<T> RetryAsync<T>(Func<Task<T>> func, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retries the specified synchronous function according to the configured strategy.
    /// </summary>
    /// <typeparam name="T">The return type of the function.</typeparam>
    /// <param name="func">The synchronous function to retry.</param>
    /// <returns>The result of the function.</returns>
    T Retry<T>(Func<T> func);
}
