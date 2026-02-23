// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.CommandLine;

namespace GraphRag.Cli.Commands;

/// <summary>
/// Creates the "index" command that runs the indexing pipeline.
/// </summary>
public static class IndexCommand
{
    /// <summary>Creates the index command.</summary>
    /// <returns>The configured index <see cref="Command"/>.</returns>
    public static Command Create()
    {
        var rootOption = new Option<string>("--root") { Description = "Root directory for the project." };
        rootOption.DefaultValueFactory = _ => ".";

        var methodOption = new Option<string>("--method") { Description = "Indexing method to use." };
        methodOption.DefaultValueFactory = _ => "standard";

        var verboseOption = new Option<bool>("--verbose") { Description = "Enable verbose output." };
        verboseOption.DefaultValueFactory = _ => false;

        var command = new Command("index", "Run the GraphRAG indexing pipeline.")
        {
            rootOption,
            methodOption,
            verboseOption,
        };

        command.SetAction(parseResult =>
        {
            var root = parseResult.GetValue(rootOption) ?? ".";
            var method = parseResult.GetValue(methodOption) ?? "standard";
            var verbose = parseResult.GetValue(verboseOption);

            // Stub: will call PipelineRunner once wired up.
            Console.WriteLine($"Running indexing pipeline (method={method}, root={root}, verbose={verbose})");
        });

        return command;
    }
}
