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

namespace GraphRag.Tests.Unit.Index;

public class PipelineRunnerTests
{
    private static readonly string[] ExpectedSteps = ["step1", "step2"];

    [Fact]
    public async Task RunPipelineAsync_ExecutesWorkflows()
    {
        var executed = new List<string>();

        var workflows = new List<(string Name, Func<GraphRagConfig, PipelineRunContext, Task<WorkflowFunctionOutput>> Function)>
        {
            ("step1", (_, _) =>
            {
                executed.Add("step1");
                return Task.FromResult(new WorkflowFunctionOutput(Result: "result1"));
            }),
            ("step2", (_, _) =>
            {
                executed.Add("step2");
                return Task.FromResult(new WorkflowFunctionOutput(Result: "result2"));
            }),
        };

        var pipeline = new Pipeline(workflows);
        var config = new GraphRagConfig();
        var context = RunContextFactory.Create(
            new Mock<IStorage>().Object,
            new Mock<ICache>().Object,
            new NoopWorkflowCallbacks());

        var results = new List<PipelineRunResult>();
        await foreach (var result in PipelineRunner.RunPipelineAsync(pipeline, config, context))
        {
            results.Add(result);
        }

        executed.Should().BeEquivalentTo(ExpectedSteps);
        results.Should().HaveCount(2);
        results[0].Workflow.Should().Be("step1");
        results[0].Error.Should().BeNull();
        results[1].Workflow.Should().Be("step2");
        results[1].Error.Should().BeNull();
    }
}
