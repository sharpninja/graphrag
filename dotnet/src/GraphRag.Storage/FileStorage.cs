// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Text.RegularExpressions;

namespace GraphRag.Storage;

/// <summary>
/// File-system based storage implementation.
/// </summary>
public class FileStorage : IStorage
{
    private readonly string _baseDir;
    private readonly string _encoding;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileStorage"/> class.
    /// </summary>
    /// <param name="baseDir">The base directory for file storage.</param>
    /// <param name="encoding">The encoding to use. Defaults to "utf-8".</param>
    public FileStorage(string baseDir, string encoding = "utf-8")
    {
        _baseDir = Path.GetFullPath(baseDir);
        _encoding = encoding;
        Directory.CreateDirectory(_baseDir);
    }

    /// <inheritdoc/>
    public IEnumerable<string> Find(Regex filePattern)
    {
        var allFiles = Directory.EnumerateFiles(_baseDir, "*", SearchOption.AllDirectories);
        foreach (var file in allFiles)
        {
            if (filePattern.IsMatch(file))
            {
                var relative = Path.GetRelativePath(_baseDir, file);
                yield return relative;
            }
        }
    }

    /// <inheritdoc/>
    public async Task<object?> GetAsync(string key, bool? asBytes = null, string? encoding = null, CancellationToken cancellationToken = default)
    {
        var filePath = JoinPath(_baseDir, key);
        if (!await HasAsync(key, cancellationToken).ConfigureAwait(false))
        {
            return null;
        }

        return await ReadFileAsync(filePath, asBytes ?? false, encoding).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task SetAsync(string key, object value, string? encoding = null, CancellationToken cancellationToken = default)
    {
        var filePath = JoinPath(_baseDir, key);
        var directory = Path.GetDirectoryName(filePath);
        if (directory is not null)
        {
            Directory.CreateDirectory(directory);
        }

        if (value is byte[] bytes)
        {
            await File.WriteAllBytesAsync(filePath, bytes, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            var enc = System.Text.Encoding.GetEncoding(encoding ?? _encoding);
            await File.WriteAllTextAsync(filePath, value?.ToString() ?? string.Empty, enc, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public Task<bool> HasAsync(string key, CancellationToken cancellationToken = default)
    {
        var filePath = JoinPath(_baseDir, key);
        return Task.FromResult(File.Exists(filePath));
    }

    /// <inheritdoc/>
    public Task DeleteAsync(string key, CancellationToken cancellationToken = default)
    {
        var filePath = JoinPath(_baseDir, key);
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public Task ClearAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in new DirectoryInfo(_baseDir).EnumerateFileSystemInfos())
        {
            if (entry is DirectoryInfo dir)
            {
                dir.Delete(recursive: true);
            }
            else
            {
                entry.Delete();
            }
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public IStorage Child(string? name)
    {
        if (name is null)
        {
            return this;
        }

        var childPath = Path.Combine(_baseDir, name);
        return new FileStorage(childPath, _encoding);
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> Keys()
    {
        return Directory.EnumerateFiles(_baseDir)
            .Select(Path.GetFileName)
            .Where(n => n is not null)
            .Cast<string>()
            .ToList();
    }

    /// <inheritdoc/>
    public Task<string> GetCreationDateAsync(string key, CancellationToken cancellationToken = default)
    {
        var filePath = JoinPath(_baseDir, key);
        var creationTimeUtc = File.GetCreationTimeUtc(filePath);
        return Task.FromResult(IStorage.GetTimestampFormattedWithLocalTz(creationTimeUtc));
    }

    private static string JoinPath(string basePath, string fileName)
    {
        return Path.GetFullPath(Path.Combine(basePath, fileName));
    }

    private async Task<object> ReadFileAsync(string path, bool asBytes, string? encoding)
    {
        if (asBytes)
        {
            return await File.ReadAllBytesAsync(path).ConfigureAwait(false);
        }

        var enc = System.Text.Encoding.GetEncoding(encoding ?? _encoding);
        return await File.ReadAllTextAsync(path, enc).ConfigureAwait(false);
    }
}
