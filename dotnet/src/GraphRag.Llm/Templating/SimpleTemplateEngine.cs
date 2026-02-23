// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm.Templating;

/// <summary>
/// A simple template engine that performs <c>{{variable}}</c> replacement using string substitution.
/// For full Jinja2-compatible templating, use Scriban-based implementations.
/// </summary>
public sealed class SimpleTemplateEngine : ITemplateEngine
{
    /// <inheritdoc />
    public string Render(string templateContent, Dictionary<string, object?> variables)
    {
        ArgumentNullException.ThrowIfNull(templateContent);
        ArgumentNullException.ThrowIfNull(variables);

        var result = templateContent;
        foreach (var (key, value) in variables)
        {
            result = result.Replace("{{" + key + "}}", value?.ToString() ?? string.Empty);
        }

        return result;
    }
}
