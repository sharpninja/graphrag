// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Llm.Types;

namespace GraphRag.Llm.Embedding;

/// <summary>
/// A mock LLM embedding provider that returns deterministic random embeddings.
/// Uses a seeded random number generator for reproducible results.
/// </summary>
public sealed class MockLlmEmbedding : ILlmEmbedding
{
    private readonly int _dimensions;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockLlmEmbedding"/> class.
    /// </summary>
    /// <param name="dimensions">The number of dimensions for generated embedding vectors.</param>
    /// <param name="tokenizer">The tokenizer to associate with this provider.</param>
    public MockLlmEmbedding(int dimensions, ITokenizer tokenizer)
    {
        ArgumentNullException.ThrowIfNull(tokenizer);

        _dimensions = dimensions;
        Tokenizer = tokenizer;
    }

    /// <inheritdoc />
    public ITokenizer Tokenizer { get; }

    /// <inheritdoc />
    public Task<LlmEmbeddingResponse> EmbedAsync(LlmEmbeddingArgs args, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(Embed(args));
    }

    /// <inheritdoc />
    public LlmEmbeddingResponse Embed(LlmEmbeddingArgs args)
    {
        var embeddings = new List<IReadOnlyList<float>>(args.Input.Count);
        var totalTokens = 0;

        foreach (var input in args.Input)
        {
            var seed = input.GetHashCode();
            var rng = new Random(seed);
            var dims = args.Dimensions ?? _dimensions;
            var vector = new float[dims];

            for (var i = 0; i < dims; i++)
            {
                vector[i] = (float)((rng.NextDouble() * 2.0) - 1.0);
            }

            embeddings.Add(vector);
            totalTokens += Tokenizer.CountTokens(input);
        }

        return new LlmEmbeddingResponse(
            Embeddings: embeddings,
            Usage: new LlmUsage(
                PromptTokens: totalTokens,
                CompletionTokens: 0,
                TotalTokens: totalTokens));
    }
}
