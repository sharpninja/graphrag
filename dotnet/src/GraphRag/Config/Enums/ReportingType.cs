// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Config.Enums;

/// <summary>
/// Builtin reporting implementation types.
/// </summary>
public static class ReportingType
{
    /// <summary>Gets the identifier for file-based reporting.</summary>
    public const string File = "file";

    /// <summary>Gets the identifier for Azure Blob reporting.</summary>
    public const string Blob = "blob";
}
