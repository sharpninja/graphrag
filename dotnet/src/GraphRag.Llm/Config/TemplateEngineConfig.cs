// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm.Config;

/// <summary>
/// Configuration for the template engine.
/// </summary>
public sealed record TemplateEngineConfig
{
    /// <summary>Gets the template engine type.</summary>
    public string Type { get; init; } = TemplateEngineType.Jinja;

    /// <summary>Gets the template manager type.</summary>
    public string ManagerType { get; init; } = TemplateManagerType.File;

    /// <summary>Gets the base path for template files.</summary>
    public string? BasePath { get; init; }
}
