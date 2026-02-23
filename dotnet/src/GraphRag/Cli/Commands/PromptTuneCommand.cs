// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.CommandLine;

namespace GraphRag.Cli.Commands;

/// <summary>
/// Creates the "prompt-tune" command that generates tuned indexing prompts.
/// </summary>
public static class PromptTuneCommand
{
    /// <summary>Creates the prompt-tune command.</summary>
    /// <returns>The configured prompt-tune <see cref="Command"/>.</returns>
    public static Command Create()
    {
        var rootOption = new Option<string>("--root") { Description = "Root directory for the project." };
        rootOption.DefaultValueFactory = _ => ".";

        var outputOption = new Option<string>("--output") { Description = "Output directory for generated prompts." };
        outputOption.DefaultValueFactory = _ => "prompts";

        var command = new Command("prompt-tune", "Generate tuned indexing prompts for the corpus.")
        {
            rootOption,
            outputOption,
        };

        command.SetAction(parseResult =>
        {
            var root = parseResult.GetValue(rootOption) ?? ".";
            var output = parseResult.GetValue(outputOption) ?? "prompts";

            // Stub: will call PromptTuner once wired up.
            Console.WriteLine($"Running prompt tuning (root={root}, output={output})");
        });

        return command;
    }
}
