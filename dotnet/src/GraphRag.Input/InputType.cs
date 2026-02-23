// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Input;

/// <summary>
/// Defines the supported input file types.
/// </summary>
public static class InputType
{
    /// <summary>CSV file type.</summary>
    public const string Csv = "csv";

    /// <summary>Plain text file type.</summary>
    public const string Text = "text";

    /// <summary>JSON file type.</summary>
    public const string Json = "json";

    /// <summary>JSON Lines file type.</summary>
    public const string JsonLines = "jsonl";

    /// <summary>MarkItDown file type.</summary>
    public const string MarkItDown = "markitdown";
}
