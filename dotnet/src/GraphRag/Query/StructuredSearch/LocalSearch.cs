// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Diagnostics;
using System.Runtime.CompilerServices;

using GraphRag.Llm;
using GraphRag.Llm.Types;
using GraphRag.Query.ContextBuilder;

namespace GraphRag.Query.StructuredSearch;

/// <summary>
/// Implements local search using entity-centric context and LLM completion.
/// </summary>
public class LocalSearch : ISearch
{
    private readonly ILlmCompletion llm;
    private readonly ILocalContextBuilder contextBuilder;
    private readonly ITokenizer tokenizer;
    private readonly int maxDataTokens;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalSearch"/> class.
    /// </summary>
    /// <param name="llm">The LLM completion provider.</param>
    /// <param name="contextBuilder">The local context builder.</param>
    /// <param name="tokenizer">The tokenizer for token counting.</param>
    /// <param name="maxDataTokens">The maximum number of data tokens for context.</param>
    public LocalSearch(
        ILlmCompletion llm,
        ILocalContextBuilder contextBuilder,
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

        var context = contextBuilder.BuildContext(query, history);
        var contextText = string.Join("\n\n", context.ContextChunks);

        var messages = new List<LlmMessage>
        {
            new("system", "You are a helpful assistant answering questions using the provided context."),
            new("user", $"Context:\n{contextText}\n\nQuestion: {query}"),
        };

        var response = await llm.CompleteAsync(
            new LlmCompletionArgs(messages),
            cancellationToken).ConfigureAwait(false);

        stopwatch.Stop();

        return new SearchResult(
            Response: response.Content ?? string.Empty,
            ContextData: context.ContextRecords,
            ContextText: contextText,
            CompletionTime: stopwatch.Elapsed.TotalSeconds,
            LlmCalls: context.LlmCalls + 1,
            PromptTokens: context.PromptTokens + (response.Usage?.PromptTokens ?? 0),
            OutputTokens: context.OutputTokens + (response.Usage?.CompletionTokens ?? 0));
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
