// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Llm.Config;

/// <summary>
/// Known metrics writer types.
/// </summary>
public static class MetricsWriterType
{
    /// <summary>Log-based metrics writer.</summary>
    public const string Log = "log";

    /// <summary>File-based metrics writer.</summary>
    public const string File = "file";
}
