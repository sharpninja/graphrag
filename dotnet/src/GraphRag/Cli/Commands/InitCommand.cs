// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.CommandLine;

namespace GraphRag.Cli.Commands;

/// <summary>
/// Creates the "init" command that scaffolds a default settings.yaml in the target directory.
/// </summary>
public static class InitCommand
{
    /// <summary>Creates the init command.</summary>
    /// <returns>The configured init <see cref="Command"/>.</returns>
    public static Command Create()
    {
        var rootOption = new Option<string>("--root") { Description = "Root directory for the project." };
        rootOption.DefaultValueFactory = _ => ".";

        var command = new Command("init", "Initialize a new GraphRAG project with default settings.")
        {
            rootOption,
        };

        command.SetAction(parseResult =>
        {
            var root = parseResult.GetValue(rootOption) ?? ".";
            var fullPath = Path.GetFullPath(root);
            Directory.CreateDirectory(fullPath);

            var settingsPath = Path.Combine(fullPath, "settings.yaml");
            Console.WriteLine($"Creating default settings.yaml in {fullPath}");

            // Stub: write minimal placeholder settings file.
            File.WriteAllText(settingsPath, "# GraphRAG default settings\n");
        });

        return command;
    }
}
