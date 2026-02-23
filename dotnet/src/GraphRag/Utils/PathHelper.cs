// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Utils;

/// <summary>
/// Provides helper methods for file system path operations.
/// </summary>
public static class PathHelper
{
    /// <summary>
    /// Determines whether a file exists at the specified path.
    /// </summary>
    /// <param name="path">The file path to check.</param>
    /// <returns><c>true</c> if the file exists; otherwise, <c>false</c>.</returns>
    public static bool FileExists(string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        return File.Exists(path);
    }

    /// <summary>
    /// Determines whether a directory exists at the specified path.
    /// </summary>
    /// <param name="path">The directory path to check.</param>
    /// <returns><c>true</c> if the directory exists; otherwise, <c>false</c>.</returns>
    public static bool DirectoryExists(string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        return Directory.Exists(path);
    }
}
