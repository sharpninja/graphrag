// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Llm.Types;

namespace GraphRag.Llm.Completion;

/// <summary>
/// A mock LLM completion provider that returns pre-configured responses in order.
/// Cycles through the response list when all responses have been used.
/// </summary>
public sealed class MockLlmCompletion : ILlmCompletion
{
    private readonly IReadOnlyList<string> _responses;
    private int _index;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockLlmCompletion"/> class.
    /// </summary>
    /// <param name="responses">The list of mock responses to return in order.</param>
    /// <param name="tokenizer">The tokenizer to associate with this provider.</param>
    public MockLlmCompletion(IReadOnlyList<string> responses, ITokenizer tokenizer)
    {
        ArgumentNullException.ThrowIfNull(responses);
        ArgumentNullException.ThrowIfNull(tokenizer);

        _responses = responses;
        Tokenizer = tokenizer;
    }

    /// <inheritdoc />
    public ITokenizer Tokenizer { get; }

    /// <inheritdoc />
    public Task<LlmCompletionResponse> CompleteAsync(LlmCompletionArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Complete(args));
    }

    /// <inheritdoc />
    public LlmCompletionResponse Complete(LlmCompletionArgs args)
    {
        var content = _responses[_index % _responses.Count];
        _index++;

        var promptTokens = 0;
        foreach (var msg in args.Messages)
        {
            promptTokens += Tokenizer.CountTokens(msg.Content);
        }

        var completionTokens = Tokenizer.CountTokens(content);

        return new LlmCompletionResponse(
            Content: content,
            Usage: new LlmUsage(
                PromptTokens: promptTokens,
                CompletionTokens: completionTokens,
                TotalTokens: promptTokens + completionTokens));
    }
}
