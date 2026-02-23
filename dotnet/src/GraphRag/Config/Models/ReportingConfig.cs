// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Config.Enums;

namespace GraphRag.Config.Models;

/// <summary>
/// Configuration for pipeline reporting.
/// </summary>
public sealed record ReportingConfig
{
    /// <summary>Gets the reporting type to use.</summary>
    public string Type { get; init; } = ReportingType.File;

    /// <summary>Gets the base directory for report output.</summary>
    public string BaseDir { get; init; } = "logs";

    /// <summary>Gets the connection string for remote reporting services.</summary>
    public string? ConnectionString { get; init; }

    /// <summary>Gets the Azure Blob Storage container name.</summary>
    public string? ContainerName { get; init; }

    /// <summary>Gets the account URL for Azure services.</summary>
    public string? AccountUrl { get; init; }
}
