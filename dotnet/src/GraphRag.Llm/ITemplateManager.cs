// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm;

/// <summary>
/// Interface for managing and retrieving templates by name.
/// </summary>
public interface ITemplateManager
{
    /// <summary>
    /// Gets the template content for the specified template name.
    /// </summary>
    /// <param name="templateName">The name of the template to retrieve.</param>
    /// <returns>The template content.</returns>
    string GetTemplate(string templateName);
}
