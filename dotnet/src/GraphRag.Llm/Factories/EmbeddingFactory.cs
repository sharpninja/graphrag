// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Common.Factories;
using GraphRag.Llm.Config;
using GraphRag.Llm.Embedding;
using GraphRag.Llm.Tokenizer;

namespace GraphRag.Llm.Factories;

/// <summary>
/// Factory for creating <see cref="ILlmEmbedding"/> instances by strategy name.
/// </summary>
public sealed class EmbeddingFactory : ServiceFactory<ILlmEmbedding>
{
    private bool _builtinsRegistered;

    /// <summary>
    /// Registers the built-in embedding providers if not already registered.
    /// </summary>
    public void EnsureBuiltins()
    {
        if (_builtinsRegistered)
        {
            return;
        }

        Register(
            LlmProviderType.MockLlm,
            args =>
            {
                var dimensions = args.TryGetValue("dimensions", out var d) && d is int dim ? dim : 256;
                var tokenizer = args.TryGetValue("tokenizer", out var t) && t is ITokenizer tok
                    ? tok
                    : new SimpleTokenizer();
                return new MockLlmEmbedding(dimensions, tokenizer);
            },
            ServiceScope.Singleton);

        _builtinsRegistered = true;
    }

    /// <summary>
    /// Creates an <see cref="ILlmEmbedding"/> instance from the specified model configuration.
    /// </summary>
    /// <param name="config">The model configuration.</param>
    /// <param name="initArgs">Optional additional initialization arguments.</param>
    /// <returns>An <see cref="ILlmEmbedding"/> instance.</returns>
    public ILlmEmbedding CreateEmbedding(ModelConfig config, Dictionary<string, object?>? initArgs = null)
    {
        EnsureBuiltins();
        return Create(config.Type, initArgs);
    }
}
