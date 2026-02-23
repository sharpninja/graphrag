// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm.Templating;

/// <summary>
/// A template manager that reads template files from a base directory.
/// </summary>
public sealed class FileTemplateManager : ITemplateManager
{
    private readonly string _baseDir;
    private readonly string _extension;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileTemplateManager"/> class.
    /// </summary>
    /// <param name="baseDir">The base directory containing template files.</param>
    /// <param name="extension">The file extension for template files (default is ".txt").</param>
    public FileTemplateManager(string baseDir, string extension = ".txt")
    {
        ArgumentNullException.ThrowIfNull(baseDir);

        _baseDir = baseDir;
        _extension = extension;
    }

    /// <inheritdoc />
    public string GetTemplate(string templateName)
    {
        ArgumentNullException.ThrowIfNull(templateName);

        var fileName = templateName.EndsWith(_extension, StringComparison.OrdinalIgnoreCase)
            ? templateName
            : templateName + _extension;

        var path = Path.Combine(_baseDir, fileName);

        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Template '{templateName}' not found at '{path}'.", path);
        }

        return File.ReadAllText(path);
    }
}
