using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace PocketPal.Tray;

/// <summary>
/// Manages the system tray icon. Since the pet window itself is
/// click-through and has no chrome, the tray icon is the only way the
/// user interacts with app-level actions (exit, and future settings/pause).
/// </summary>
public sealed class TrayIconManager : IDisposable
{
    private readonly NotifyIcon _notifyIcon;

    public event Action? ExitRequested;

    public TrayIconManager()
    {
        _notifyIcon = new NotifyIcon
        {
            Icon = LoadIcon(),
            Visible = true,
            Text = "Pocket Pal"
        };

        var menu = new ContextMenuStrip();
        menu.Items.Add("Pocket Pal", null, (_, _) => { }).Enabled = false;
        menu.Items.Add(new ToolStripSeparator());
        menu.Items.Add("Exit", null, (_, _) => ExitRequested?.Invoke());

        _notifyIcon.ContextMenuStrip = menu;
    }

    private static Icon LoadIcon()
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Assets", "tray.ico");

        if (File.Exists(path))
            return new Icon(path);

        // Fall back to a generic system icon so the app still runs even if
        // no custom tray icon has been added yet.
        return SystemIcons.Application;
    }

    public void Dispose()
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
    }
}
