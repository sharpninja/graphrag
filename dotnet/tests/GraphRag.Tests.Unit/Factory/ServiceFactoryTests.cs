// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using GraphRag.Common.Factories;

namespace GraphRag.Tests.Unit.Factory;

public class TestService
{
    public string Name { get; }

    public TestService(string name = "default")
    {
        Name = name;
    }
}

public class TestFactory : ServiceFactory<TestService>
{
}

public class ServiceFactoryTests
{
    [Fact]
    public void Register_And_Create_TransientService()
    {
        var factory = new TestFactory();
        factory.Register("test", args =>
        {
            var name = args.TryGetValue("name", out var n) ? n?.ToString() ?? "default" : "default";
            return new TestService(name);
        });

        var service = factory.Create("test", new Dictionary<string, object?> { ["name"] = "hello" });
        Assert.Equal("hello", service.Name);
    }

    [Fact]
    public void Create_ThrowsForUnregisteredStrategy()
    {
        var factory = new TestFactory();
        Assert.Throws<InvalidOperationException>(() => factory.Create("unknown"));
    }

    [Fact]
    public void Contains_ReturnsTrueForRegistered()
    {
        var factory = new TestFactory();
        factory.Register("test", _ => new TestService());
        Assert.True(factory.Contains("test"));
        Assert.False(factory.Contains("other"));
    }

    [Fact]
    public void Keys_ReturnsRegisteredStrategies()
    {
        var factory = new TestFactory();
        factory.Register("a", _ => new TestService());
        factory.Register("b", _ => new TestService());
        Assert.Contains("a", factory.Keys);
        Assert.Contains("b", factory.Keys);
    }

    [Fact]
    public void Singleton_CachesSameInstance()
    {
        var factory = new TestFactory();
        factory.Register("test", _ => new TestService(), ServiceScope.Singleton);

        var s1 = factory.Create("test");
        var s2 = factory.Create("test");
        Assert.Same(s1, s2);
    }

    [Fact]
    public void Transient_CreatesNewInstance()
    {
        var factory = new TestFactory();
        factory.Register("test", _ => new TestService(), ServiceScope.Transient);

        var s1 = factory.Create("test");
        var s2 = factory.Create("test");
        Assert.NotSame(s1, s2);
    }

    [Fact]
    public void NullArgs_AreFiltered()
    {
        var factory = new TestFactory();
        factory.Register("test", args =>
        {
            Assert.DoesNotContain("null_key", args.Keys);
            return new TestService();
        });

        factory.Create("test", new Dictionary<string, object?> { ["null_key"] = null, ["real"] = "value" });
    }
}
