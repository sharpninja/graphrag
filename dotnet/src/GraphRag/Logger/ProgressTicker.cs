// Copyright (c) 2025 Microsoft Corporation.
// Licensed under the MIT License

namespace GraphRag.Logger;

/// <summary>
/// Tracks progress by counting completed items against a total and invoking a callback on each tick.
/// </summary>
public sealed class ProgressTicker
{
    private readonly Action<ProgressInfo> _callback;
    private readonly int? _totalItems;
    private int _completedItems;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProgressTicker"/> class.
    /// </summary>
    /// <param name="callback">The callback to invoke on each tick.</param>
    /// <param name="totalItems">The optional total number of items to process.</param>
    public ProgressTicker(Action<ProgressInfo> callback, int? totalItems = null)
    {
        _callback = callback ?? throw new ArgumentNullException(nameof(callback));
        _totalItems = totalItems;
    }

    /// <summary>
    /// Gets the number of completed items.
    /// </summary>
    public int CompletedItems => _completedItems;

    /// <summary>
    /// Gets the total number of items, if known.
    /// </summary>
    public int? TotalItems => _totalItems;

    /// <summary>
    /// Advances the progress by the specified count and invokes the callback.
    /// </summary>
    /// <param name="count">The number of items to advance by.</param>
    public void Tick(int count = 1)
    {
        _completedItems += count;
        _callback(new ProgressInfo
        {
            CompletedItems = _completedItems,
            TotalItems = _totalItems,
        });
    }

    /// <summary>
    /// Signals that the operation is complete and invokes the callback with the final state.
    /// </summary>
    public void Done()
    {
        _callback(new ProgressInfo
        {
            CompletedItems = _completedItems,
            TotalItems = _totalItems,
            Description = "Done",
        });
    }
}
