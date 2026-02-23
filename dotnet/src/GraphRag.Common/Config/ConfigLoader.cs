// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Text.Json;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace GraphRag.Common.Config;

/// <summary>
/// Options for <see cref="ConfigLoader.LoadConfig{T}"/>.
/// </summary>
public sealed record ConfigLoaderOptions
{
    /// <summary>Gets path to config directory or file. Defaults to current directory.</summary>
    public string? ConfigPath { get; init; }

    /// <summary>Gets configuration overrides to merge on top of file values.</summary>
    public Dictionary<string, object?>? Overrides { get; init; }

    /// <summary>Gets a value indicating whether to set CWD to the config file's directory.</summary>
    public bool SetCwd { get; init; } = true;

    /// <summary>Gets a value indicating whether to substitute $VAR environment variables in config text.</summary>
    public bool ParseEnvVars { get; init; } = true;

    /// <summary>Gets a value indicating whether to load a .env file before substituting env vars.</summary>
    public bool LoadDotEnvFile { get; init; } = true;

    /// <summary>Gets explicit .env file path. If null, looks for .env next to config file.</summary>
    public string? DotEnvPath { get; init; }

    /// <summary>Gets file encoding. Defaults to UTF-8.</summary>
    public string FileEncoding { get; init; } = "utf-8";
}

/// <summary>
/// Loads configuration from YAML/JSON files with environment variable substitution.
/// </summary>
public static partial class ConfigLoader
{
    private static readonly string[] DefaultConfigFiles = ["settings.yaml", "settings.yml", "settings.json"];

    /// <summary>
    /// Load configuration from a file and initialize it with the given factory function.
    /// </summary>
    /// <typeparam name="T">The configuration type to produce.</typeparam>
    /// <param name="initializer">Factory function that takes a dictionary and returns T.</param>
    /// <param name="options">Configuration loading options.</param>
    /// <returns>The initialized configuration object.</returns>
    public static T LoadConfig<T>(
        Func<Dictionary<string, object?>, T> initializer,
        ConfigLoaderOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(initializer);
        options ??= new ConfigLoaderOptions();

        var configPath = ResolveConfigPath(options.ConfigPath);
        var fileContents = File.ReadAllText(configPath, System.Text.Encoding.GetEncoding(options.FileEncoding));

        if (options.ParseEnvVars)
        {
            if (options.LoadDotEnvFile)
            {
                var required = options.DotEnvPath is not null;
                var dotEnvPath = options.DotEnvPath ?? Path.Combine(Path.GetDirectoryName(configPath)!, ".env");
                LoadDotEnv(dotEnvPath, required);
            }

            fileContents = ParseEnvVariables(fileContents);
        }

        var parser = GetParserForFile(configPath);
        Dictionary<string, object?> configData;

        try
        {
            configData = parser(fileContents);
        }
        catch (Exception ex) when (ex is not ConfigParsingError)
        {
            throw new ConfigParsingError($"Failed to parse config_path: {configPath}. Error: {ex.Message}", ex);
        }

        if (options.Overrides is not null)
        {
            try
            {
                RecursiveMergeDicts(configData, options.Overrides);
            }
            catch (Exception ex) when (ex is not ConfigParsingError)
            {
                throw new ConfigParsingError($"Failed to merge overrides with config_path: {configPath}. Error: {ex.Message}", ex);
            }
        }

        if (options.SetCwd)
        {
            Directory.SetCurrentDirectory(Path.GetDirectoryName(configPath)!);
        }

        return initializer(configData);
    }

    /// <summary>
    /// Recursively merge source dictionary into destination dictionary.
    /// </summary>
    /// <param name="dest">The destination dictionary.</param>
    /// <param name="src">The source dictionary to merge from.</param>
    public static void RecursiveMergeDicts(Dictionary<string, object?> dest, Dictionary<string, object?> src)
    {
        ArgumentNullException.ThrowIfNull(dest);
        ArgumentNullException.ThrowIfNull(src);

        foreach (var (key, value) in src)
        {
            if (value is Dictionary<string, object?> srcDict
                && dest.TryGetValue(key, out var existing)
                && existing is Dictionary<string, object?> destDict)
            {
                RecursiveMergeDicts(destDict, srcDict);
            }
            else
            {
                dest[key] = value;
            }
        }
    }

    private static string ResolveConfigPath(string? configDirOrFile)
    {
        var path = configDirOrFile is not null
            ? Path.GetFullPath(configDirOrFile)
            : Directory.GetCurrentDirectory();

        if (File.Exists(path))
        {
            return path;
        }

        if (!Directory.Exists(path))
        {
            throw new FileNotFoundException($"Invalid config path: {path} is not a directory");
        }

        foreach (var file in DefaultConfigFiles)
        {
            var candidate = Path.Combine(path, file);
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        throw new FileNotFoundException($"No 'settings.[yaml|yml|json]' config file found in directory: {path}");
    }

    private static void LoadDotEnv(string dotEnvPath, bool required)
    {
        if (!File.Exists(dotEnvPath))
        {
            if (!required)
            {
                return;
            }

            throw new FileNotFoundException($"dot_env_path not found: {dotEnvPath}");
        }

        foreach (var line in File.ReadAllLines(dotEnvPath))
        {
            var trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith('#'))
            {
                continue;
            }

            var eqIndex = trimmed.IndexOf('=', StringComparison.Ordinal);
            if (eqIndex <= 0)
            {
                continue;
            }

            var key = trimmed[..eqIndex].Trim();
            var value = trimmed[(eqIndex + 1)..].Trim().Trim('"', '\'');
            Environment.SetEnvironmentVariable(key, value);
        }
    }

    private static string ParseEnvVariables(string text)
    {
        return EnvVarPattern().Replace(text, match =>
        {
            var varName = match.Groups[1].Value;
            var envValue = Environment.GetEnvironmentVariable(varName);
            return envValue ?? throw new ConfigParsingError($"Environment variable not found: '{varName}'");
        });
    }

    private static Func<string, Dictionary<string, object?>> GetParserForFile(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToUpperInvariant();
        return extension switch
        {
            ".JSON" => ParseJson,
            ".YAML" or ".YML" => ParseYaml,
            _ => throw new ConfigParsingError(
                $"Failed to parse, {filePath}. Unsupported file extension, {extension}. "
                + "Use one of the supported file extensions: .json, .yaml, .yml."),
        };
    }

    private static Dictionary<string, object?> ParseJson(string data)
    {
        return JsonSerializer.Deserialize<Dictionary<string, object?>>(data) ?? [];
    }

    private static Dictionary<string, object?> ParseYaml(string data)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .Build();
        return deserializer.Deserialize<Dictionary<string, object?>>(data) ?? [];
    }

    [GeneratedRegex(@"\$\{?(\w+)\}?")]
    private static partial Regex EnvVarPattern();
}
