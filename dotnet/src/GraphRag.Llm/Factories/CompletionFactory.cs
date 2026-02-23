// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Common.Factories;
using GraphRag.Llm.Completion;
using GraphRag.Llm.Config;
using GraphRag.Llm.Tokenizer;

namespace GraphRag.Llm.Factories;

/// <summary>
/// Factory for creating <see cref="ILlmCompletion"/> instances by strategy name.
/// </summary>
public sealed class CompletionFactory : ServiceFactory<ILlmCompletion>
{
    private bool _builtinsRegistered;

    /// <summary>
    /// Registers the built-in completion providers if not already registered.
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
                var responses = args.TryGetValue("responses", out var r) && r is IReadOnlyList<string> list
                    ? list
                    : (IReadOnlyList<string>)["mock response"];
                var tokenizer = args.TryGetValue("tokenizer", out var t) && t is ITokenizer tok
                    ? tok
                    : new SimpleTokenizer();
                return new MockLlmCompletion(responses, tokenizer);
            },
            ServiceScope.Singleton);

        _builtinsRegistered = true;
    }

    /// <summary>
    /// Creates an <see cref="ILlmCompletion"/> instance from the specified model configuration.
    /// </summary>
    /// <param name="config">The model configuration.</param>
    /// <param name="initArgs">Optional additional initialization arguments.</param>
    /// <returns>An <see cref="ILlmCompletion"/> instance.</returns>
    public ILlmCompletion CreateCompletion(ModelConfig config, Dictionary<string, object?>? initArgs = null)
    {
        EnsureBuiltins();
        return Create(config.Type, initArgs);
    }
}
