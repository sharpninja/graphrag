// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Reflection;

namespace GraphRag.Prompts;

/// <summary>
/// Provides access to prompt templates stored as embedded resources.
/// </summary>
public static class PromptResources
{
    private static readonly Assembly ResourceAssembly = typeof(PromptResources).Assembly;

    /// <summary>
    /// Gets the prompt template content by name.
    /// </summary>
    /// <param name="promptName">
    /// The name of the prompt (e.g., "ExtractGraph", "SummarizeDescriptions").
    /// Do not include the .txt extension.
    /// </param>
    /// <returns>The prompt template content.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the prompt resource is not found.</exception>
    public static string GetPrompt(string promptName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(promptName);

        // Try embedded resource first
        var resourceName = $"GraphRag.Prompts.{promptName}.txt";
        using var stream = ResourceAssembly.GetManifestResourceStream(resourceName);
        if (stream is not null)
        {
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        // Fall back to file path (useful for development/customization)
        var assemblyDir = Path.GetDirectoryName(ResourceAssembly.Location) ?? ".";
        var filePath = Path.Combine(assemblyDir, "Prompts", $"{promptName}.txt");
        if (File.Exists(filePath))
        {
            return File.ReadAllText(filePath);
        }

        throw new FileNotFoundException(
            $"Prompt template '{promptName}' not found as embedded resource '{resourceName}' or file '{filePath}'.");
    }

    /// <summary>
    /// Gets the prompt template content by name, or <c>null</c> if not found.
    /// </summary>
    /// <param name="promptName">
    /// The name of the prompt (e.g., "ExtractGraph", "SummarizeDescriptions").
    /// </param>
    /// <returns>The prompt template content, or <c>null</c> if not found.</returns>
    public static string? GetPromptOrDefault(string promptName)
    {
        try
        {
            return GetPrompt(promptName);
        }
        catch (FileNotFoundException)
        {
            return null;
        }
    }

    /// <summary>Well-known prompt name for entity/relationship extraction.</summary>
    public const string ExtractGraph = "ExtractGraph";

    /// <summary>Well-known prompt name for description summarization.</summary>
    public const string SummarizeDescriptions = "SummarizeDescriptions";

    /// <summary>Well-known prompt name for community report generation.</summary>
    public const string CommunityReport = "CommunityReport";

    /// <summary>Well-known prompt name for text-based community report generation.</summary>
    public const string CommunityReportText = "CommunityReportText";

    /// <summary>Well-known prompt name for claim extraction.</summary>
    public const string ExtractClaims = "ExtractClaims";

    /// <summary>Well-known prompt name for global search map step.</summary>
    public const string GlobalSearchMap = "GlobalSearchMap";

    /// <summary>Well-known prompt name for global search reduce step.</summary>
    public const string GlobalSearchReduce = "GlobalSearchReduce";

    /// <summary>Well-known prompt name for local search.</summary>
    public const string LocalSearch = "LocalSearch";

    /// <summary>Well-known prompt name for drift search.</summary>
    public const string DriftSearch = "DriftSearch";

    /// <summary>Well-known prompt name for basic search.</summary>
    public const string BasicSearch = "BasicSearch";
}
