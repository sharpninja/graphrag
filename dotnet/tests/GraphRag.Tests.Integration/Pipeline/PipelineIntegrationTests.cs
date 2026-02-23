// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;

using GraphRag.Cache;
using GraphRag.Callbacks;
using GraphRag.Config.Models;
using GraphRag.Index.Run;
using GraphRag.Index.Typing;
using GraphRag.Storage;

using Moq;

namespace GraphRag.Tests.Integration.Pipeline;

public class PipelineIntegrationTests
{
    [Fact]
    public async Task RunPipeline_WithStubWorkflows_ProducesResults()
    {
        // Arrange
        var workflows = new List<(string Name, Func<GraphRagConfig, PipelineRunContext, Task<WorkflowFunctionOutput>> Function)>
        {
            ("step1", (_, _) => Task.FromResult(new WorkflowFunctionOutput("result1"))),
            ("step2", (_, _) => Task.FromResult(new WorkflowFunctionOutput("result2"))),
            ("step3", (_, _) => Task.FromResult(new WorkflowFunctionOutput("result3"))),
        };

        var pipeline = new Index.Typing.Pipeline(workflows);
        var config = new GraphRagConfig();
        var context = new PipelineRunContext
        {
            Stats = new PipelineRunStats(0, 0, 0, 0, new Dictionary<string, WorkflowMetrics>()),
            Storage = new MemoryStorage(),
            Cache = new MemoryCache(),
            Callbacks = new Mock<IWorkflowCallbacks>().Object,
            State = new Dictionary<string, object?>(),
        };

        // Act
        var results = new List<PipelineRunResult>();
        await foreach (var result in PipelineRunner.RunPipelineAsync(pipeline, config, context))
        {
            results.Add(result);
        }

        // Assert
        results.Should().HaveCount(3);
        results.Select(r => r.Workflow).Should().ContainInOrder("step1", "step2", "step3");
        results.Should().OnlyContain(r => r.Error == null);
        results[0].Result.Should().Be("result1");
        results[1].Result.Should().Be("result2");
        results[2].Result.Should().Be("result3");
    }
}
