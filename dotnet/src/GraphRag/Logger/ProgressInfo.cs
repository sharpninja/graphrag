// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Logger;

/// <summary>
/// Represents progress information for a workflow operation.
/// </summary>
public sealed record ProgressInfo
{
    /// <summary>
    /// Gets the description of the current progress step.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Gets the total number of items to process.
    /// </summary>
    public int? TotalItems { get; init; }

    /// <summary>
    /// Gets the number of items that have been completed.
    /// </summary>
    public int? CompletedItems { get; init; }
}
