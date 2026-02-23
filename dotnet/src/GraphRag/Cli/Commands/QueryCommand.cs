// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.CommandLine;

namespace GraphRag.Cli.Commands;

/// <summary>
/// Creates the "query" command that executes a search query.
/// </summary>
public static class QueryCommand
{
    /// <summary>Creates the query command.</summary>
    /// <returns>The configured query <see cref="Command"/>.</returns>
    public static Command Create()
    {
        var rootOption = new Option<string>("--root") { Description = "Root directory for the project." };
        rootOption.DefaultValueFactory = _ => ".";

        var methodOption = new Option<string>("--method") { Description = "Search method to use.", Required = true };
        methodOption.AcceptOnlyFromAmong("local", "global", "drift", "basic");

        var queryOption = new Option<string>("--query") { Description = "The query string to execute.", Required = true };

        var command = new Command("query", "Run a GraphRAG search query.")
        {
            rootOption,
            methodOption,
            queryOption,
        };

        command.SetAction(parseResult =>
        {
            var root = parseResult.GetValue(rootOption) ?? ".";
            var method = parseResult.GetValue(methodOption) ?? "local";
            var query = parseResult.GetValue(queryOption) ?? string.Empty;

            // Stub: will call appropriate search engine once wired up.
            Console.WriteLine($"Executing {method} search in {root}: \"{query}\"");
        });

        return command;
    }
}
