// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Llm;
using GraphRag.Query.ContextBuilder;
using GraphRag.Query.StructuredSearch;

namespace GraphRag.Query;

/// <summary>
/// Factory for creating search engine instances.
/// </summary>
public static class QueryFactory
{
    /// <summary>
    /// Creates a global search engine.
    /// </summary>
    /// <param name="llm">The LLM completion provider.</param>
    /// <param name="contextBuilder">The global context builder.</param>
    /// <param name="tokenizer">The tokenizer for token counting.</param>
    /// <param name="maxDataTokens">The maximum number of data tokens per context chunk.</param>
    /// <returns>A configured global search engine.</returns>
    public static ISearch GetGlobalSearchEngine(
        ILlmCompletion llm,
        IGlobalContextBuilder contextBuilder,
        ITokenizer tokenizer,
        int maxDataTokens = 8000)
    {
        return new GlobalSearch(llm, contextBuilder, tokenizer, maxDataTokens);
    }

    /// <summary>
    /// Creates a local search engine.
    /// </summary>
    /// <param name="llm">The LLM completion provider.</param>
    /// <param name="contextBuilder">The local context builder.</param>
    /// <param name="tokenizer">The tokenizer for token counting.</param>
    /// <param name="maxDataTokens">The maximum number of data tokens for context.</param>
    /// <returns>A configured local search engine.</returns>
    public static ISearch GetLocalSearchEngine(
        ILlmCompletion llm,
        ILocalContextBuilder contextBuilder,
        ITokenizer tokenizer,
        int maxDataTokens = 8000)
    {
        return new LocalSearch(llm, contextBuilder, tokenizer, maxDataTokens);
    }

    /// <summary>
    /// Creates a basic search engine.
    /// </summary>
    /// <param name="llm">The LLM completion provider.</param>
    /// <param name="contextBuilder">The basic context builder.</param>
    /// <param name="tokenizer">The tokenizer for token counting.</param>
    /// <param name="maxDataTokens">The maximum number of data tokens for context.</param>
    /// <returns>A configured basic search engine.</returns>
    public static ISearch GetBasicSearchEngine(
        ILlmCompletion llm,
        IBasicContextBuilder contextBuilder,
        ITokenizer tokenizer,
        int maxDataTokens = 8000)
    {
        return new BasicSearch(llm, contextBuilder, tokenizer, maxDataTokens);
    }

    /// <summary>
    /// Creates a DRIFT search engine.
    /// </summary>
    /// <param name="llm">The LLM completion provider.</param>
    /// <param name="contextBuilder">The DRIFT context builder.</param>
    /// <param name="tokenizer">The tokenizer for token counting.</param>
    /// <param name="maxIterations">The maximum number of refinement iterations.</param>
    /// <returns>A configured DRIFT search engine.</returns>
    public static ISearch GetDriftSearchEngine(
        ILlmCompletion llm,
        IDriftContextBuilder contextBuilder,
        ITokenizer tokenizer,
        int maxIterations = 5)
    {
        return new DriftSearch(llm, contextBuilder, tokenizer, maxIterations);
    }
}
