// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Common.Discovery;
using Scriban;
using Scriban.Runtime;

namespace GraphRag.Llm.Scriban;

/// <summary>
/// A template engine implementation backed by the Scriban templating library.
/// </summary>
[StrategyImplementation("scriban", typeof(ITemplateEngine))]
public sealed class ScribanTemplateEngine : ITemplateEngine
{
    /// <inheritdoc />
    public string Render(string templateContent, Dictionary<string, object?> variables)
    {
        ArgumentNullException.ThrowIfNull(templateContent);
        ArgumentNullException.ThrowIfNull(variables);

        var template = Template.Parse(templateContent);
        if (template.HasErrors)
        {
            throw new InvalidOperationException(
                $"Template parse errors: {string.Join("; ", template.Messages)}");
        }

        var scriptObject = new ScriptObject();
        foreach (var (key, value) in variables)
        {
            scriptObject.Add(key, value);
        }

        var context = new TemplateContext();
        context.PushGlobal(scriptObject);

        return template.Render(context);
    }
}
