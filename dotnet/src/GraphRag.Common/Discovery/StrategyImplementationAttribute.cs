// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Common.Discovery;

/// <summary>
/// Marks a class as a strategy implementation discoverable at runtime.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class StrategyImplementationAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="StrategyImplementationAttribute"/> class.
    /// </summary>
    /// <param name="strategyKey">The strategy type key (e.g., "blob", "cosmosdb").</param>
    /// <param name="interfaceType">The interface this class implements.</param>
    public StrategyImplementationAttribute(string strategyKey, Type interfaceType)
    {
        StrategyKey = strategyKey;
        InterfaceType = interfaceType;
    }

    /// <summary>
    /// Gets the strategy type key (e.g., "blob", "cosmosdb").
    /// </summary>
    public string StrategyKey { get; }

    /// <summary>
    /// Gets the interface this class implements.
    /// </summary>
    public Type InterfaceType { get; }
}
