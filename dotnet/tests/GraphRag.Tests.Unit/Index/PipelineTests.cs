// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Config.Models;
using GraphRag.Index.Typing;
using GraphRag.Index.Workflows;

namespace GraphRag.Tests.Unit.Index;

public class PipelineTests
{
    private static readonly string[] TwoNames = ["wf1", "wf2"];
    private static readonly string[] ThreeMinusOne = ["wf1", "wf3"];

    [Fact]
    public void Pipeline_Names_ReturnsWorkflowNames()
    {
        var workflows = new List<(string Name, Func<GraphRagConfig, PipelineRunContext, Task<WorkflowFunctionOutput>> Function)>
        {
            ("wf1", StubAsync),
            ("wf2", StubAsync),
        };

        var pipeline = new Pipeline(workflows);

        pipeline.Names.Should().BeEquivalentTo(TwoNames);
    }

    [Fact]
    public void Pipeline_Remove_RemovesWorkflow()
    {
        var workflows = new List<(string Name, Func<GraphRagConfig, PipelineRunContext, Task<WorkflowFunctionOutput>> Function)>
        {
            ("wf1", StubAsync),
            ("wf2", StubAsync),
            ("wf3", StubAsync),
        };

        var pipeline = new Pipeline(workflows);
        pipeline.Remove("wf2");

        pipeline.Names.Should().BeEquivalentTo(ThreeMinusOne);
    }

    [Fact]
    public void Pipeline_Run_YieldsWorkflows()
    {
        var workflows = new List<(string Name, Func<GraphRagConfig, PipelineRunContext, Task<WorkflowFunctionOutput>> Function)>
        {
            ("wf1", StubAsync),
            ("wf2", StubAsync),
        };

        var pipeline = new Pipeline(workflows);
        var yielded = pipeline.Run().Select(w => w.Name).ToList();

        yielded.Should().BeEquivalentTo(TwoNames);
    }

    private static Task<WorkflowFunctionOutput> StubAsync(GraphRagConfig config, PipelineRunContext context) =>
        Task.FromResult(new WorkflowFunctionOutput(Result: null));
}
