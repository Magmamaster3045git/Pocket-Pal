using System;
using System.Runtime.InteropServices;

namespace PocketPal.Native;

/// <summary>
/// Thin wrapper around the handful of Win32 calls needed to turn a normal
/// WPF window into a click-through overlay: mouse events pass straight
/// through to whatever app is beneath the pet, and the window never steals
/// focus, appears in Alt+Tab, or shows a taskbar button.
/// </summary>
internal static class NativeMethods
{
    private const int GWL_EXSTYLE = -20;

    private const int WS_EX_TRANSPARENT = 0x00000020; // click-through
    private const int WS_EX_LAYERED = 0x00080000;      // required for transparency + click-through
    private const int WS_EX_TOOLWINDOW = 0x00000080;   // hide from taskbar
    private const int WS_EX_NOACTIVATE = 0x08000000;   // never steals keyboard focus

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    private static extern bool SetWindowPos(
        IntPtr hWnd,
        IntPtr hWndInsertAfter,
        int X,
        int Y,
        int cx,
        int cy,
        uint uFlags);
    
    
    public static void KeepWindowAboveTaskbar(IntPtr hwnd)
    {
        const uint SWP_NOSIZE = 0x0001;
        const uint SWP_NOMOVE = 0x0002;
        const uint SWP_NOACTIVATE = 0x0010;
    
        SetWindowPos(
            hwnd,
            new IntPtr(-1), // HWND_TOPMOST
            0,
            0,
            0,
            0,
            SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE
        );
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

    /// <summary>Applies click-through + no-activate + tool-window styles to the given window handle.</summary>
    public static void MakeClickThrough(IntPtr hwnd)
    {
        int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
        exStyle |= WS_EX_TRANSPARENT | WS_EX_LAYERED | WS_EX_TOOLWINDOW | WS_EX_NOACTIVATE;
        SetWindowLong(hwnd, GWL_EXSTYLE, exStyle);
    }

    /// <summary>
    /// Reverts click-through so the window can receive mouse input again.
    /// Reserved for a future "interactive mode" toggle (petting, dragging, etc.).
    /// </summary>
    public static void MakeInteractive(IntPtr hwnd)
    {
        int exStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
        exStyle &= ~WS_EX_TRANSPARENT;
        SetWindowLong(hwnd, GWL_EXSTYLE, exStyle);
    }
}
