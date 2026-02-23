// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Llm.Types;

namespace GraphRag.Llm.Threading;

/// <summary>
/// Static helper that runs multiple completion requests concurrently with configurable concurrency.
/// </summary>
public static class CompletionBatchRunner
{
    /// <summary>
    /// Runs multiple completion requests concurrently using the specified completion provider.
    /// </summary>
    /// <param name="completion">The completion provider to use.</param>
    /// <param name="requests">The list of completion request arguments.</param>
    /// <param name="maxConcurrency">The maximum number of concurrent requests.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>A list of completion responses in the same order as the requests.</returns>
    public static async Task<IReadOnlyList<LlmCompletionResponse>> RunAsync(
        ILlmCompletion completion,
        IReadOnlyList<LlmCompletionArgs> requests,
        int maxConcurrency = 4,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(completion);
        ArgumentNullException.ThrowIfNull(requests);

        var results = new LlmCompletionResponse[requests.Count];
        using var semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);

        var tasks = new Task[requests.Count];
        for (var i = 0; i < requests.Count; i++)
        {
            var index = i;
            tasks[i] = Task.Run(
                async () =>
                {
                    await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
                    try
                    {
                        results[index] = await completion.CompleteAsync(requests[index], cancellationToken).ConfigureAwait(false);
                    }
                    finally
                    {
                        semaphore.Release();
                    }
                },
                cancellationToken);
        }

        await Task.WhenAll(tasks).ConfigureAwait(false);
        return results;
    }
}
