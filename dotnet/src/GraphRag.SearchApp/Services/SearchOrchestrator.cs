// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Diagnostics;
using GraphRag.Query;
using GraphRag.SearchApp.Models;
using Microsoft.Extensions.Logging;

namespace GraphRag.SearchApp.Services;

/// <summary>
/// Orchestrates parallel execution of multiple search types.
/// </summary>
public class SearchOrchestrator
{
    private readonly ILogger<SearchOrchestrator> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="SearchOrchestrator"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public SearchOrchestrator(ILogger<SearchOrchestrator> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Runs the specified search types in parallel against the given search engines.
    /// </summary>
    /// <param name="query">The search query.</param>
    /// <param name="engines">A dictionary mapping search type to the corresponding search engine.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The list of search results, one per engine.</returns>
    public async Task<IReadOnlyList<AppSearchResult>> RunSearchesAsync(
        string query,
        IReadOnlyDictionary<SearchType, ISearch> engines,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(query);
        ArgumentNullException.ThrowIfNull(engines);

        _logger.LogInformation("Running {Count} search(es) for query: {Query}", engines.Count, query);

        var tasks = engines.Select(kvp => RunSingleSearchAsync(kvp.Key, kvp.Value, query, cancellationToken));
        var results = await Task.WhenAll(tasks).ConfigureAwait(false);

        return results;
    }

    private async Task<AppSearchResult> RunSingleSearchAsync(
        SearchType type,
        ISearch engine,
        string query,
        CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            var result = await engine.SearchAsync(query, cancellationToken: cancellationToken).ConfigureAwait(false);
            sw.Stop();

            _logger.LogInformation("{Type} search completed in {Time:F2}s.", type, sw.Elapsed.TotalSeconds);

            return new AppSearchResult(
                Type: type,
                Response: result.Response,
                CompletionTime: result.CompletionTime > 0 ? result.CompletionTime : sw.Elapsed.TotalSeconds,
                LlmCalls: result.LlmCalls,
                PromptTokens: result.PromptTokens,
                OutputTokens: result.OutputTokens,
                ContextData: result.ContextData);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            sw.Stop();
            _logger.LogError(ex, "{Type} search failed after {Time:F2}s.", type, sw.Elapsed.TotalSeconds);

            return new AppSearchResult(
                Type: type,
                Response: $"Error: {ex.Message}",
                CompletionTime: sw.Elapsed.TotalSeconds);
        }
    }
}
