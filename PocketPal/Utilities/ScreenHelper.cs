using System.Windows.Forms;
using Microsoft.Win32;

namespace PocketPal.Utilities;

/// <summary>
/// Wraps WinForms' Screen API (still the simplest reliable way to get
/// physical monitor bounds/work areas in a WPF app) and exposes a
/// resolution/monitor-change event so the main window can reposition
/// itself automatically.
/// </summary>
public sealed class ScreenHelper : IDisposable
{
    public event Action? DisplaySettingsChanged;

    public ScreenHelper()
    {
        SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;
    }

    /// <summary>
    /// Returns the work area (excludes taskbar) of the requested monitor
    /// index, or the primary monitor if the index is out of range / -1.
    /// </summary>
    public System.Drawing.Rectangle GetWorkArea(int preferredMonitorIndex)
    {
        var screens = Screen.AllScreens;

        if (preferredMonitorIndex >= 0 && preferredMonitorIndex < screens.Length)
            return screens[preferredMonitorIndex].WorkingArea;

        return Screen.PrimaryScreen?.WorkingArea ?? screens[0].WorkingArea;
    }

    public int MonitorCount => Screen.AllScreens.Length;

    private void OnDisplaySettingsChanged(object? sender, EventArgs e)
    {
        DisplaySettingsChanged?.Invoke();
    }

    public void Dispose()
    {
        SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;
    }
}
