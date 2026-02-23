// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Reflection;

namespace GraphRag.Common.Discovery;

/// <summary>
/// Discovers strategy implementations from assemblies using the
/// <see cref="StrategyImplementationAttribute"/> marker.
/// </summary>
public sealed class StrategyDiscovery
{
    private readonly List<StrategyDescriptor> _discovered = [];

    /// <summary>
    /// Gets all discovered strategy descriptors.
    /// </summary>
    public IReadOnlyList<StrategyDescriptor> Descriptors => _discovered;

    /// <summary>
    /// Scan all assemblies matching the GraphRag.*.dll convention in the application directory.
    /// </summary>
    /// <returns>The total number of strategies discovered.</returns>
    public int DiscoverFromApplicationDirectory()
    {
        var baseDir = AppContext.BaseDirectory;
        var files = Directory.GetFiles(baseDir, "GraphRag.*.dll");
        var count = 0;

        foreach (var file in files)
        {
            try
            {
                count += DiscoverAllStrategies(file);
            }
            catch (Exception)
            {
                // Skip assemblies that fail to load (e.g., native, resource-only).
            }
        }

        return count;
    }

    /// <summary>
    /// Register all strategies from a <see cref="StrategyConfiguration"/>.
    /// </summary>
    /// <param name="config">The strategy configuration.</param>
    /// <returns>The total number of strategies discovered.</returns>
    public int RegisterFromConfiguration(StrategyConfiguration config)
    {
        var count = 0;

        foreach (var assemblyName in config.Assemblies)
        {
            count += DiscoverAllStrategies(assemblyName);
        }

        return count;
    }

    /// <summary>
    /// Scan an assembly for types decorated with <see cref="StrategyImplementationAttribute"/>
    /// that implement <typeparamref name="TInterface"/>.
    /// </summary>
    /// <typeparam name="TInterface">The interface type to scan for.</typeparam>
    /// <param name="assembly">The assembly to scan.</param>
    /// <returns>The number of strategies discovered.</returns>
    public int DiscoverStrategies<TInterface>(Assembly assembly)
    {
        return DiscoverStrategies(typeof(TInterface), assembly);
    }

    /// <summary>
    /// Scan an assembly for types decorated with <see cref="StrategyImplementationAttribute"/>
    /// that implement the given interface type.
    /// </summary>
    /// <param name="interfaceType">The interface type to scan for.</param>
    /// <param name="assembly">The assembly to scan.</param>
    /// <returns>The number of strategies discovered.</returns>
    public int DiscoverStrategies(Type interfaceType, Assembly assembly)
    {
        var count = 0;
        foreach (var type in assembly.GetExportedTypes())
        {
            var attr = type.GetCustomAttribute<StrategyImplementationAttribute>();
            if (attr is null || attr.InterfaceType != interfaceType)
            {
                continue;
            }

            if (!interfaceType.IsAssignableFrom(type))
            {
                continue;
            }

            _discovered.Add(new StrategyDescriptor(
                attr.StrategyKey,
                attr.InterfaceType,
                type,
                assembly));

            count++;
        }

        return count;
    }

    /// <summary>
    /// Load an assembly by name and scan it for strategy implementations.
    /// </summary>
    /// <param name="assemblyName">The assembly name or path.</param>
    /// <returns>The number of strategies discovered.</returns>
    public int DiscoverAllStrategies(string assemblyName)
    {
        var assembly = LoadAssembly(nameOrPath: assemblyName);
        if (assembly is null)
        {
            return 0;
        }

        var count = 0;
        foreach (var type in assembly.GetExportedTypes())
        {
            var attr = type.GetCustomAttribute<StrategyImplementationAttribute>();
            if (attr is null)
            {
                continue;
            }

            if (!attr.InterfaceType.IsAssignableFrom(type))
            {
                continue;
            }

            _discovered.Add(new StrategyDescriptor(
                attr.StrategyKey,
                attr.InterfaceType,
                type,
                assembly));

            count++;
        }

        return count;
    }

    /// <summary>
    /// Find all discovered descriptors for a given interface type.
    /// </summary>
    /// <typeparam name="TInterface">The interface to find strategies for.</typeparam>
    /// <returns>The matching descriptors.</returns>
    public IReadOnlyList<StrategyDescriptor> GetDescriptors<TInterface>()
    {
        return _discovered
            .Where(d => d.InterfaceType == typeof(TInterface))
            .ToList();
    }

    /// <summary>
    /// Find a descriptor by interface type and strategy key.
    /// </summary>
    /// <typeparam name="TInterface">The interface type.</typeparam>
    /// <param name="strategyKey">The strategy key.</param>
    /// <returns>The descriptor, or null if not found.</returns>
    public StrategyDescriptor? GetDescriptor<TInterface>(string strategyKey)
    {
        return _discovered.FirstOrDefault(
            d => d.InterfaceType == typeof(TInterface) && d.StrategyKey == strategyKey);
    }

    private static Assembly? LoadAssembly(string nameOrPath)
    {
        try
        {
            if (File.Exists(nameOrPath))
            {
                return Assembly.LoadFrom(nameOrPath);
            }

            return Assembly.Load(nameOrPath);
        }
        catch (Exception)
        {
            return null;
        }
    }
}
