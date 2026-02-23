// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Reflection;
using FluentAssertions;
using GraphRag.Common.Discovery;
using GraphRag.Common.Factories;

namespace GraphRag.Tests.Unit.Discovery;

/// <summary>
/// Test interface for strategy discovery tests.
/// </summary>
public interface ITestService
{
}

/// <summary>
/// Fake implementation discovered via attribute.
/// </summary>
[StrategyImplementation("test_impl", typeof(ITestService))]
public class FakeTestService : ITestService
{
}

/// <summary>
/// Second fake to verify multiple discoveries.
/// </summary>
[StrategyImplementation("test_impl_alt", typeof(ITestService))]
public class AltFakeTestService : ITestService
{
}

/// <summary>
/// Concrete factory for testing RegisterFromDiscovery.
/// </summary>
public class DiscoveryTestServiceFactory : ServiceFactory<ITestService>
{
}

/// <summary>
/// Unit tests for <see cref="StrategyImplementationAttribute"/>,
/// <see cref="StrategyDiscovery"/>, <see cref="StrategyConfiguration"/>,
/// and <see cref="ServiceFactory{T}.RegisterFromDiscovery"/>.
/// </summary>
public class StrategyDiscoveryTests
{
    [Fact]
    public void Attribute_SetsPropertiesCorrectly()
    {
        var attr = new StrategyImplementationAttribute("my_key", typeof(ITestService));

        attr.StrategyKey.Should().Be("my_key");
        attr.InterfaceType.Should().Be<ITestService>();
    }

    [Fact]
    public void Attribute_IsAppliedToFakeTestService()
    {
        var attr = typeof(FakeTestService)
            .GetCustomAttribute<StrategyImplementationAttribute>();

        attr.Should().NotBeNull();
        attr!.StrategyKey.Should().Be("test_impl");
        attr.InterfaceType.Should().Be<ITestService>();
    }

    [Fact]
    public void DiscoverStrategies_FindsAnnotatedTypes()
    {
        var discovery = new StrategyDiscovery();
        var count = discovery.DiscoverStrategies<ITestService>(
            Assembly.GetExecutingAssembly());

        count.Should().BeGreaterThanOrEqualTo(2);
        discovery.Descriptors.Should().HaveCountGreaterThanOrEqualTo(2);
    }

    [Fact]
    public void DiscoverStrategies_IgnoresUnrelatedInterfaces()
    {
        var discovery = new StrategyDiscovery();
        var count = discovery.DiscoverStrategies<IDisposable>(
            Assembly.GetExecutingAssembly());

        count.Should().Be(0);
    }

    [Fact]
    public void GetDescriptors_ReturnsMatchingDescriptors()
    {
        var discovery = new StrategyDiscovery();
        discovery.DiscoverStrategies<ITestService>(Assembly.GetExecutingAssembly());

        var descriptors = discovery.GetDescriptors<ITestService>();

        descriptors.Should().HaveCountGreaterThanOrEqualTo(2);
        descriptors.Should().Contain(d => d.StrategyKey == "test_impl");
        descriptors.Should().Contain(d => d.StrategyKey == "test_impl_alt");
    }

    [Fact]
    public void GetDescriptors_ReturnsEmptyForUnknownInterface()
    {
        var discovery = new StrategyDiscovery();
        discovery.DiscoverStrategies<ITestService>(Assembly.GetExecutingAssembly());

        var descriptors = discovery.GetDescriptors<IDisposable>();

        descriptors.Should().BeEmpty();
    }

    [Fact]
    public void GetDescriptor_ReturnsCorrectDescriptor()
    {
        var discovery = new StrategyDiscovery();
        discovery.DiscoverStrategies<ITestService>(Assembly.GetExecutingAssembly());

        var descriptor = discovery.GetDescriptor<ITestService>("test_impl");

        descriptor.Should().NotBeNull();
        descriptor!.StrategyKey.Should().Be("test_impl");
        descriptor.InterfaceType.Should().Be<ITestService>();
        descriptor.ImplementationType.Should().Be<FakeTestService>();
        descriptor.SourceAssembly.Should().BeSameAs(Assembly.GetExecutingAssembly());
    }

    [Fact]
    public void GetDescriptor_ReturnsNullForUnknownKey()
    {
        var discovery = new StrategyDiscovery();
        discovery.DiscoverStrategies<ITestService>(Assembly.GetExecutingAssembly());

        var descriptor = discovery.GetDescriptor<ITestService>("nonexistent");

        descriptor.Should().BeNull();
    }

    [Fact]
    public void RegisterFromDiscovery_RegistersDiscoveredStrategies()
    {
        var discovery = new StrategyDiscovery();
        discovery.DiscoverStrategies<ITestService>(Assembly.GetExecutingAssembly());

        var factory = new DiscoveryTestServiceFactory();
        var count = factory.RegisterFromDiscovery(discovery);

        count.Should().BeGreaterThanOrEqualTo(2);
        factory.Contains("test_impl").Should().BeTrue();
        factory.Contains("test_impl_alt").Should().BeTrue();
    }

    [Fact]
    public void RegisterFromDiscovery_CreatedInstanceHasCorrectType()
    {
        var discovery = new StrategyDiscovery();
        discovery.DiscoverStrategies<ITestService>(Assembly.GetExecutingAssembly());

        var factory = new DiscoveryTestServiceFactory();
        factory.RegisterFromDiscovery(discovery);

        var service = factory.Create("test_impl");

        service.Should().BeOfType<FakeTestService>();
    }

    [Fact]
    public void RegisterFromDiscovery_SkipsAlreadyRegistered()
    {
        var discovery = new StrategyDiscovery();
        discovery.DiscoverStrategies<ITestService>(Assembly.GetExecutingAssembly());

        var factory = new DiscoveryTestServiceFactory();
        factory.Register("test_impl", _ => new FakeTestService());

        var count = factory.RegisterFromDiscovery(discovery);

        count.Should().Be(discovery.GetDescriptors<ITestService>().Count - 1);
    }

    [Fact]
    public void StrategyConfiguration_HasEmptyDefaults()
    {
        var config = new StrategyConfiguration();

        config.Defaults.Should().BeEmpty();
        config.Assemblies.Should().BeEmpty();
        config.Overrides.Should().BeEmpty();
    }

    [Fact]
    public void StrategyConfiguration_AcceptsValues()
    {
        var config = new StrategyConfiguration
        {
            Defaults = new() { ["storage"] = "blob" },
            Assemblies = ["GraphRag.Storage"],
            Overrides = new()
            {
                ["custom"] = new StrategyOverride
                {
                    Assembly = "My.Assembly",
                    Type = "My.Assembly.CustomImpl",
                },
            },
        };

        config.Defaults.Should().ContainKey("storage").WhoseValue.Should().Be("blob");
        config.Assemblies.Should().ContainSingle().Which.Should().Be("GraphRag.Storage");
        config.Overrides.Should().ContainKey("custom");
        config.Overrides["custom"].Assembly.Should().Be("My.Assembly");
        config.Overrides["custom"].Type.Should().Be("My.Assembly.CustomImpl");
    }

    [Fact]
    public void StrategyOverride_DefaultsToNull()
    {
        var over = new StrategyOverride();

        over.Assembly.Should().BeNull();
        over.Type.Should().BeNull();
    }
}
