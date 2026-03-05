// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

using System.Runtime.CompilerServices;
using System.Text.Json;

using GraphRag.Callbacks;
using GraphRag.Config.Models;
using GraphRag.Index.Typing;

namespace GraphRag.Index.Run;

/// <summary>
/// Executes an indexing pipeline and yields results for each workflow.
/// </summary>
public static class PipelineRunner
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
    };

    /// <summary>
    /// Runs all workflows in the given pipeline, yielding a result for each one.
    /// </summary>
    /// <param name="pipeline">The pipeline to execute.</param>
    /// <param name="config">The GraphRAG configuration.</param>
    /// <param name="context">The pipeline run context.</param>
    /// <param name="callbacks">Optional additional workflow callbacks.</param>
    /// <param name="isUpdateRun">Whether this is an incremental update run.</param>
    /// <param name="ct">A token to cancel the operation.</param>
    /// <returns>An async stream of <see cref="PipelineRunResult"/> for each executed workflow.</returns>
    public static async IAsyncEnumerable<PipelineRunResult> RunPipelineAsync(
        Pipeline pipeline,
        GraphRagConfig config,
        PipelineRunContext context,
        IEnumerable<IWorkflowCallbacks>? callbacks = null,
        bool isUpdateRun = false,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        var callbackList = callbacks?.ToList() ?? [];

        // Notify all callbacks that the pipeline is starting.
        foreach (var cb in callbackList)
        {
            cb.OnPipelineStart(pipeline.Names);
        }

        var results = new List<object>();

        // Initial stats dump before workflows start, matching Python behavior.
        // This creates the stats.json file so monitoring tools can detect pipeline startup.
        await DumpStatsAsync(context, ct).ConfigureAwait(false);

        foreach (var (name, function) in pipeline.Run())
        {
            ct.ThrowIfCancellationRequested();

            PipelineRunResult result;
            Exception? error = null;
            bool stop = false;

            try
            {
                foreach (var cb in callbackList)
                {
                    cb.OnWorkflowStart(name, function);
                }

                using var profiler = new WorkflowProfiler();
                var output = await function(config, context).ConfigureAwait(false);

                // Capture metrics after disposal.
                profiler.Dispose();
                if (profiler.Metrics is not null)
                {
                    context.Stats.Workflows[name] = profiler.Metrics;
                }

                if (output.Result is not null)
                {
                    results.Add(output.Result);
                }

                result = new PipelineRunResult(
                    Workflow: name,
                    Result: output.Result,
                    State: context.State,
                    Error: null);

                foreach (var cb in callbackList)
                {
                    cb.OnWorkflowEnd(name, function);
                }

                stop = output.Stop;
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                error = ex;
                foreach (var cb in callbackList)
                {
                    cb.OnPipelineError(ex);
                }

                result = new PipelineRunResult(
                    Workflow: name,
                    Result: null,
                    State: context.State,
                    Error: ex);
            }

            // Dump stats after each workflow, matching Python's per-workflow stats persistence.
            await DumpStatsAsync(context, ct).ConfigureAwait(false);

            yield return result;

            if (error is not null || stop)
            {
                break;
            }
        }

        // Notify all callbacks that the pipeline has ended.
        foreach (var cb in callbackList)
        {
            cb.OnPipelineEnd(results);
        }
    }

    /// <summary>
    /// Serializes the current pipeline run statistics to the output storage as <c>stats.json</c>.
    /// </summary>
    /// <param name="context">The pipeline run context containing stats and storage.</param>
    /// <param name="ct">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    internal static async Task DumpStatsAsync(PipelineRunContext context, CancellationToken ct = default)
    {
        var statsObject = new Dictionary<string, object?>
        {
            ["total_runtime"] = context.Stats.TotalRuntime,
            ["num_documents"] = context.Stats.NumDocuments,
            ["update_documents"] = context.Stats.UpdateDocuments,
            ["input_load_time"] = context.Stats.InputLoadTime,
            ["workflows"] = context.Stats.Workflows.ToDictionary(
                kvp => kvp.Key,
                kvp => (object?)new Dictionary<string, object?>
                {
                    ["overall_time_seconds"] = kvp.Value.OverallTimeSeconds,
                    ["peak_memory_bytes"] = kvp.Value.PeakMemoryBytes,
                    ["memory_delta_bytes"] = kvp.Value.MemoryDeltaBytes,
                }),
        };

        var json = JsonSerializer.Serialize(statsObject, JsonOptions);

        await context.Storage.SetAsync("stats.json", json, cancellationToken: ct).ConfigureAwait(false);
    }
}
