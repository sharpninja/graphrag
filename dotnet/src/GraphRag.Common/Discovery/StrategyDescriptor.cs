// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Reflection;

namespace GraphRag.Common.Discovery;

/// <summary>
/// Describes a discovered strategy implementation.
/// </summary>
/// <param name="StrategyKey">The strategy key (e.g., "blob", "cosmosdb").</param>
/// <param name="InterfaceType">The interface type this strategy implements.</param>
/// <param name="ImplementationType">The concrete implementation type.</param>
/// <param name="SourceAssembly">The assembly containing the implementation.</param>
public sealed record StrategyDescriptor(
    string StrategyKey,
    Type InterfaceType,
    Type ImplementationType,
    Assembly SourceAssembly);
