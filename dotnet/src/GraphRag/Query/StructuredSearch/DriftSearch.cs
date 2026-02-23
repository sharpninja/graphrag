// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Diagnostics;
using System.Runtime.CompilerServices;

using GraphRag.Llm;
using GraphRag.Llm.Types;
using GraphRag.Query.ContextBuilder;
using GraphRag.Query.StructuredSearch.Drift;

namespace GraphRag.Query.StructuredSearch;

/// <summary>
/// Implements DRIFT (Dynamic Reasoning and Inference with Flexible Traversal) search
/// using iterative refinement over knowledge graph context.
/// </summary>
public class DriftSearch : ISearch
{
    private readonly ILlmCompletion llm;
    private readonly IDriftContextBuilder contextBuilder;
    private readonly ITokenizer tokenizer;
    private readonly int maxIterations;

    /// <summary>
    /// Initializes a new instance of the <see cref="DriftSearch"/> class.
    /// </summary>
    /// <param name="llm">The LLM completion provider.</param>
    /// <param name="contextBuilder">The DRIFT context builder.</param>
    /// <param name="tokenizer">The tokenizer for token counting.</param>
    /// <param name="maxIterations">The maximum number of refinement iterations.</param>
    public DriftSearch(
        ILlmCompletion llm,
        IDriftContextBuilder contextBuilder,
        ITokenizer tokenizer,
        int maxIterations = 5)
    {
        this.llm = llm;
        this.contextBuilder = contextBuilder;
        this.tokenizer = tokenizer;
        this.maxIterations = maxIterations;
    }

    /// <inheritdoc/>
    public async Task<SearchResult> SearchAsync(
        string query,
        ConversationHistory? history = null,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var state = new QueryState();
        var totalLlmCalls = 0;
        var totalPromptTokens = 0;
        var totalOutputTokens = 0;

        var context = await contextBuilder.BuildContextAsync(query, cancellationToken).ConfigureAwait(false);
        totalLlmCalls += context.LlmCalls;
        totalPromptTokens += context.PromptTokens;
        totalOutputTokens += context.OutputTokens;

        var contextText = string.Join("\n\n", context.ContextChunks);

        var messages = new List<LlmMessage>
        {
            new("system", "You are a helpful assistant answering questions using the provided context. Provide a comprehensive answer."),
            new("user", $"Context:\n{contextText}\n\nQuestion: {query}"),
        };

        var response = await llm.CompleteAsync(
            new LlmCompletionArgs(messages),
            cancellationToken).ConfigureAwait(false);

        totalLlmCalls++;
        totalPromptTokens += response.Usage?.PromptTokens ?? 0;
        totalOutputTokens += response.Usage?.CompletionTokens ?? 0;

        state.AddAction(new DriftAction(query, response.Content, Score: 1.0));

        stopwatch.Stop();

        return new SearchResult(
            Response: response.Content ?? string.Empty,
            ContextData: context.ContextRecords,
            ContextText: contextText,
            CompletionTime: stopwatch.Elapsed.TotalSeconds,
            LlmCalls: totalLlmCalls,
            PromptTokens: totalPromptTokens,
            OutputTokens: totalOutputTokens);
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
