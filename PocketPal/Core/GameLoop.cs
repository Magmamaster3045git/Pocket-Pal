using System.Diagnostics;
using System.Windows.Media;

namespace PocketPal.Core;

/// <summary>
/// Drives an Update(deltaSeconds) callback once per WPF composition frame
/// (~60 Hz, vsync-aligned). This gives smooth movement while individual
/// systems (like AnimationPlayer) are free to throttle themselves to a
/// slower fixed rate internally. Kept generic/reusable - it knows nothing
/// about pets, only about calling a delegate with elapsed time.
/// </summary>
public sealed class GameLoop
{
    private readonly Stopwatch _stopwatch = new();
    private readonly Action<double> _onUpdate;
    private double _lastElapsedSeconds;
    private bool _running;

    /// <summary>
    /// Safety cap: if the app was suspended/minimized and a huge time gap
    /// occurs, clamp delta so physics doesn't "teleport" the pet.
    /// </summary>
    private const double MaxDeltaSeconds = 0.25;

    public GameLoop(Action<double> onUpdate)
    {
        _onUpdate = onUpdate;
    }

    public void Start()
    {
        if (_running) return;
        _running = true;
        _stopwatch.Restart();
        _lastElapsedSeconds = 0;
        CompositionTarget.Rendering += OnRendering;
    }

    public void Stop()
    {
        if (!_running) return;
        _running = false;
        CompositionTarget.Rendering -= OnRendering;
        _stopwatch.Stop();
    }

    private void OnRendering(object? sender, EventArgs e)
    {
        double now = _stopwatch.Elapsed.TotalSeconds;
        double delta = now - _lastElapsedSeconds;
        _lastElapsedSeconds = now;

        if (delta < 0) delta = 0;
        if (delta > MaxDeltaSeconds) delta = MaxDeltaSeconds;

        _onUpdate(delta);
    }
}
