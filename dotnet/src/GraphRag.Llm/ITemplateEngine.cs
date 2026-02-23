// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm;

/// <summary>
/// Interface for rendering templates with variable substitution.
/// </summary>
public interface ITemplateEngine
{
    /// <summary>
    /// Renders the specified template with the given variables.
    /// </summary>
    /// <param name="templateContent">The template string to render.</param>
    /// <param name="variables">The variables to substitute into the template.</param>
    /// <returns>The rendered template string.</returns>
    string Render(string templateContent, Dictionary<string, object?> variables);
}
