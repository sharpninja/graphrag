// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Common.Factories;
using GraphRag.Llm.Tokenizer;

namespace GraphRag.Llm.Factories;

/// <summary>
/// Factory for creating <see cref="ITokenizer"/> instances by strategy name.
/// </summary>
public sealed class TokenizerFactory : ServiceFactory<ITokenizer>
{
    private bool _builtinsRegistered;

    /// <summary>
    /// Registers the built-in tokenizers if not already registered.
    /// </summary>
    public void EnsureBuiltins()
    {
        if (_builtinsRegistered)
        {
            return;
        }

        Register("simple", _ => new SimpleTokenizer(), ServiceScope.Singleton);

        _builtinsRegistered = true;
    }
}
