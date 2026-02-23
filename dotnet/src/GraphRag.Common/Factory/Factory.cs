// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Common.Discovery;
using GraphRag.Common.Hasher;

namespace GraphRag.Common.Factories;

/// <summary>
/// Internal descriptor for a registered service.
/// </summary>
/// <typeparam name="T">The service type.</typeparam>
/// <param name="Scope">The service scope.</param>
/// <param name="Initializer">Factory function for the service.</param>
internal sealed record ServiceDescriptor<T>(
    ServiceScope Scope,
    Func<Dictionary<string, object?>, T> Initializer)
    where T : class;

/// <summary>
/// Abstract base class for factories that create service instances by strategy name.
/// </summary>
/// <typeparam name="T">The type of service this factory creates.</typeparam>
public abstract class ServiceFactory<T>
    where T : class
{
    private readonly Dictionary<string, ServiceDescriptor<T>> _serviceInitializers = [];
    private readonly Dictionary<string, T> _initializedServices = [];

    /// <summary>
    /// Gets a list of registered strategy names.
    /// </summary>
    public IReadOnlyList<string> Keys => _serviceInitializers.Keys.ToList();

    /// <summary>
    /// Check if a strategy is registered.
    /// </summary>
    /// <param name="strategy">The strategy name.</param>
    /// <returns>True if registered.</returns>
    public bool Contains(string strategy) => _serviceInitializers.ContainsKey(strategy);

    /// <summary>
    /// Register a new service strategy.
    /// </summary>
    /// <param name="strategy">The name of the strategy.</param>
    /// <param name="initializer">A callable that creates an instance of T from init args.</param>
    /// <param name="scope">The scope of the service (Singleton or Transient).</param>
    public void Register(
        string strategy,
        Func<Dictionary<string, object?>, T> initializer,
        ServiceScope scope = ServiceScope.Transient)
    {
        _serviceInitializers[strategy] = new ServiceDescriptor<T>(scope, initializer);
    }

    /// <summary>
    /// Register strategy implementations discovered by <see cref="StrategyDiscovery"/>.
    /// Each discovered type that implements <typeparamref name="T"/> is registered
    /// using its strategy key and instantiated via <see cref="Activator.CreateInstance(Type, object[])"/>.
    /// </summary>
    /// <param name="discovery">The discovery instance containing scanned strategies.</param>
    /// <param name="scope">The scope for discovered strategies.</param>
    /// <returns>The number of strategies registered.</returns>
    public int RegisterFromDiscovery(
        StrategyDiscovery discovery,
        ServiceScope scope = ServiceScope.Transient)
    {
        var descriptors = discovery.GetDescriptors<T>();
        var count = 0;

        foreach (var descriptor in descriptors)
        {
            var implType = descriptor.ImplementationType;

            // Skip if already registered.
            if (_serviceInitializers.ContainsKey(descriptor.StrategyKey))
            {
                continue;
            }

            Register(
                descriptor.StrategyKey,
                args => (T)Activator.CreateInstance(implType)!,
                scope);

            count++;
        }

        return count;
    }

    /// <summary>
    /// Create a service instance based on the strategy.
    /// </summary>
    /// <param name="strategy">The name of the strategy.</param>
    /// <param name="initArgs">Optional dictionary of arguments to pass to the initializer.</param>
    /// <returns>An instance of T.</returns>
    /// <exception cref="InvalidOperationException">If the strategy is not registered.</exception>
    public T Create(string strategy, Dictionary<string, object?>? initArgs = null)
    {
        if (!_serviceInitializers.TryGetValue(strategy, out var descriptor))
        {
            var registered = string.Join(", ", _serviceInitializers.Keys);
            throw new InvalidOperationException(
                $"Strategy '{strategy}' is not registered. Registered strategies are: {registered}");
        }

        // Remove entries with null values so services can use defaults
        var args = initArgs?
            .Where(kv => kv.Value is not null)
            .ToDictionary(kv => kv.Key, kv => kv.Value)
            ?? [];

        if (descriptor.Scope == ServiceScope.Singleton)
        {
            var cacheKey = HashHelper.HashData(new Dictionary<string, object?>
            {
                ["strategy"] = strategy,
                ["init_args"] = args,
            });

            if (!_initializedServices.TryGetValue(cacheKey, out var cached))
            {
                cached = descriptor.Initializer(args);
                _initializedServices[cacheKey] = cached;
            }

            return cached;
        }

        return descriptor.Initializer(args);
    }
}
