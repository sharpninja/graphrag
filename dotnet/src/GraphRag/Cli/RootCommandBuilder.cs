// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.CommandLine;

using GraphRag.Cli.Commands;

namespace GraphRag.Cli;

/// <summary>
/// Builds the root "graphrag" command with all subcommands registered.
/// </summary>
public static class RootCommandBuilder
{
    /// <summary>Builds the root command with all subcommands.</summary>
    /// <returns>The configured <see cref="RootCommand"/>.</returns>
    public static RootCommand Build()
    {
        var root = new RootCommand("GraphRAG — Graph-based Retrieval-Augmented Generation CLI");

        root.Add(InitCommand.Create());
        root.Add(IndexCommand.Create());
        root.Add(QueryCommand.Create());
        root.Add(PromptTuneCommand.Create());

        return root;
    }
}
