// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Common.Factories;
using GraphRag.Llm.Config;
using GraphRag.Llm.Templating;

namespace GraphRag.Llm.Factories;

/// <summary>
/// Factory for creating <see cref="ITemplateEngine"/> instances by strategy name.
/// </summary>
public sealed class TemplateEngineFactory : ServiceFactory<ITemplateEngine>
{
    private bool _builtinsRegistered;

    /// <summary>
    /// Registers the built-in template engines if not already registered.
    /// </summary>
    public void EnsureBuiltins()
    {
        if (_builtinsRegistered)
        {
            return;
        }

        Register(TemplateEngineType.Jinja, _ => new SimpleTemplateEngine(), ServiceScope.Singleton);

        _builtinsRegistered = true;
    }
}
