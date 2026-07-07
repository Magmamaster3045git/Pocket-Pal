using System.Drawing;
using System.Windows.Forms;
using System.IO;
using PocketPal.Settings;

namespace PocketPal.Tray;

/// <summary>
/// Manages the system tray icon.
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


        menu.Items.Add(
            "Pocket Pal",
            null,
            (_, _) => { }
        ).Enabled = false;


        menu.Items.Add(new ToolStripSeparator());


        // Settings button
        menu.Items.Add(
            "Settings",
            null,
            (_, _) =>
            {
                var settingsWindow = new SettingsWindow();
                settingsWindow.Show();
            });


        menu.Items.Add(new ToolStripSeparator());


        // Exit button
        menu.Items.Add(
            "Exit",
            null,
            (_, _) => ExitRequested?.Invoke()
        );


        _notifyIcon.ContextMenuStrip = menu;
    }



    private static Icon LoadIcon()
    {
        string path = Path.Combine(
            AppContext.BaseDirectory,
            "Assets",
            "tray.ico");


        if (File.Exists(path))
            return new Icon(path);


        return SystemIcons.Application;
    }



    public void Dispose()
    {
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
    }
}
