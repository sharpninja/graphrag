// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.SearchApp.Config;

/// <summary>
/// Describes a dataset available for searching.
/// </summary>
/// <param name="Key">The unique identifier for the dataset.</param>
/// <param name="Path">The file system or blob path to the dataset output.</param>
/// <param name="Name">The display name for the dataset.</param>
/// <param name="Description">A brief description of the dataset.</param>
/// <param name="CommunityLevel">The default community level for report filtering.</param>
public sealed record DatasetConfig(
    string Key,
    string Path,
    string Name,
    string Description,
    int CommunityLevel = 0);
