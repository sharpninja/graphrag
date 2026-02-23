// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Callbacks;

/// <summary>
/// A no-op implementation of <see cref="IQueryCallbacks"/> that discards all events.
/// </summary>
public sealed class NoopQueryCallbacks : IQueryCallbacks
{
    /// <inheritdoc />
    public void OnContext(object context)
    {
    }

    /// <inheritdoc />
    public void OnMapResponseStart(IReadOnlyList<string> contexts)
    {
    }

    /// <inheritdoc />
    public void OnMapResponseEnd(IReadOnlyList<object> outputs)
    {
    }

    /// <inheritdoc />
    public void OnReduceResponseStart(object context)
    {
    }

    /// <inheritdoc />
    public void OnReduceResponseEnd(string output)
    {
    }

    /// <inheritdoc />
    public void OnLlmNewToken(string token)
    {
    }
}
