// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Input;

/// <summary>
/// Configuration for input file reading.
/// </summary>
public sealed record InputConfig
{
    /// <summary>Gets the type of input files to read.</summary>
    public string Type { get; init; } = InputType.Text;

    /// <summary>Gets the encoding to use when reading files.</summary>
    public string? Encoding { get; init; }

    /// <summary>Gets the regex pattern to match input files.</summary>
    public string? FilePattern { get; init; }

    /// <summary>Gets the column name to use as the document ID.</summary>
    public string? IdColumn { get; init; }

    /// <summary>Gets the column name to use as the document title.</summary>
    public string? TitleColumn { get; init; }

    /// <summary>Gets the column name to use as the document text.</summary>
    public string? TextColumn { get; init; }
}
