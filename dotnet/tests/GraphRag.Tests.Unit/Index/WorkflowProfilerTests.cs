// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using FluentAssertions;
using GraphRag.Index.Run;

namespace GraphRag.Tests.Unit.Index;

public class WorkflowProfilerTests
{
    [Fact]
    public void Profiler_MeasuresElapsedTime()
    {
        WorkflowProfiler profiler;
        using (profiler = new WorkflowProfiler())
        {
            // Perform a small amount of work to ensure measurable elapsed time.
            Thread.Sleep(10);
        }

        profiler.Metrics.Should().NotBeNull();
        profiler.Metrics!.OverallTimeSeconds.Should().BeGreaterThan(0);
    }
}
