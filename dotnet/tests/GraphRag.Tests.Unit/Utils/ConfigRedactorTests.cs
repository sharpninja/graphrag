// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Utils;

namespace GraphRag.Tests.Unit.Utils;

/// <summary>
/// Unit tests for <see cref="ConfigRedactor"/>.
/// </summary>
public class ConfigRedactorTests
{
    [Fact]
    public void Redact_MasksSensitiveKeys()
    {
        var config = new Dictionary<string, object?>
        {
            { "api_key", "secret-value" },
            { "password", "my-pass" },
            { "token", "abc123" },
        };

        var result = ConfigRedactor.Redact(config);

        result["api_key"].Should().Be("***REDACTED***");
        result["password"].Should().Be("***REDACTED***");
        result["token"].Should().Be("***REDACTED***");
    }

    [Fact]
    public void Redact_LeavesNonSensitiveKeys()
    {
        var config = new Dictionary<string, object?>
        {
            { "host", "localhost" },
            { "port", 8080 },
        };

        var result = ConfigRedactor.Redact(config);

        result["host"].Should().Be("localhost");
        result["port"].Should().Be(8080);
    }
}
