// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.CommandLine;

using GraphRag.Cli;

var config = new CommandLineConfiguration(RootCommandBuilder.Build());
return await config.InvokeAsync(args).ConfigureAwait(false);
