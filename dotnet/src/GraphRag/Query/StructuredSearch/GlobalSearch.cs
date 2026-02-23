// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Diagnostics;
using System.Runtime.CompilerServices;

using GraphRag.Llm;
using GraphRag.Llm.Types;
using GraphRag.Query.ContextBuilder;

namespace GraphRag.Query.StructuredSearch;

/// <summary>
/// Implements global search using a map-reduce strategy over community reports.
/// </summary>
public class GlobalSearch : ISearch
{
    private readonly ILlmCompletion llm;
    private readonly IGlobalContextBuilder contextBuilder;
    private readonly ITokenizer tokenizer;
    private readonly int maxDataTokens;

    /// <summary>
    /// Initializes a new instance of the <see cref="GlobalSearch"/> class.
    /// </summary>
    /// <param name="llm">The LLM completion provider.</param>
    /// <param name="contextBuilder">The global context builder.</param>
    /// <param name="tokenizer">The tokenizer for token counting.</param>
    /// <param name="maxDataTokens">The maximum number of data tokens per context chunk.</param>
    public GlobalSearch(
        ILlmCompletion llm,
        IGlobalContextBuilder contextBuilder,
        ITokenizer tokenizer,
        int maxDataTokens = 8000)
    {
        this.llm = llm;
        this.contextBuilder = contextBuilder;
        this.tokenizer = tokenizer;
        this.maxDataTokens = maxDataTokens;
    }

    /// <inheritdoc/>
    public async Task<SearchResult> SearchAsync(
        string query,
        ConversationHistory? history = null,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        var context = await contextBuilder.BuildContextAsync(query, history, cancellationToken).ConfigureAwait(false);

        var totalLlmCalls = context.LlmCalls;
        var totalPromptTokens = context.PromptTokens;
        var totalOutputTokens = context.OutputTokens;

        // Map phase: process each context chunk
        var mapResults = new List<SearchResult>();
        foreach (var chunk in context.ContextChunks)
        {
            var messages = new List<LlmMessage>
            {
                new("system", "You are a helpful assistant answering questions using the provided context."),
                new("user", $"Context:\n{chunk}\n\nQuestion: {query}"),
            };

            var response = await llm.CompleteAsync(
                new LlmCompletionArgs(messages),
                cancellationToken).ConfigureAwait(false);

            totalLlmCalls++;
            totalPromptTokens += response.Usage?.PromptTokens ?? 0;
            totalOutputTokens += response.Usage?.CompletionTokens ?? 0;

            mapResults.Add(new SearchResult(
                Response: response.Content ?? string.Empty,
                LlmCalls: 1,
                PromptTokens: response.Usage?.PromptTokens ?? 0,
                OutputTokens: response.Usage?.CompletionTokens ?? 0));
        }

        // Reduce phase: combine map results
        var combinedResponses = string.Join("\n\n", mapResults.Select(r => r.Response));
        var reduceMessages = new List<LlmMessage>
        {
            new("system", "You are a helpful assistant. Synthesize the following responses into a single coherent answer."),
            new("user", $"Responses:\n{combinedResponses}\n\nOriginal question: {query}"),
        };

        var reduceResponse = await llm.CompleteAsync(
            new LlmCompletionArgs(reduceMessages),
            cancellationToken).ConfigureAwait(false);

        totalLlmCalls++;
        totalPromptTokens += reduceResponse.Usage?.PromptTokens ?? 0;
        totalOutputTokens += reduceResponse.Usage?.CompletionTokens ?? 0;

        stopwatch.Stop();

        return new GlobalSearchResult(
            Response: reduceResponse.Content ?? string.Empty,
            ContextData: context.ContextRecords,
            CompletionTime: stopwatch.Elapsed.TotalSeconds,
            LlmCalls: totalLlmCalls,
            PromptTokens: totalPromptTokens,
            OutputTokens: totalOutputTokens,
            MapResults: mapResults);
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<string> StreamSearchAsync(
        string query,
        ConversationHistory? history = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var result = await SearchAsync(query, history, cancellationToken).ConfigureAwait(false);
        yield return result.Response;
    }
}
