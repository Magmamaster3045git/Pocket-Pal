using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using PocketPal.Assets;
using PocketPal.Assets.AssetLoader;
using PocketPal.Core;
using PocketPal.Movement;
using PocketPal.Native;
using PocketPal.Rendering;
using PocketPal.Settings;
using PocketPal.Tray;
using PocketPal.Utilities;

namespace PocketPal;

public partial class MainWindow : Window
{
    private readonly SettingsManager _settingsManager = new();
    private readonly AppSettings _settings;
    private readonly ScreenHelper _screenHelper = new();
    private readonly TrayIconManager _trayIcon = new();

    private PetEngine? _engine;
    private GameLoop? _loop;

    private double _spriteScale = 2.2;
    private double _groundY;

    private DispatcherTimer? _fullscreenTimer;
    private DispatcherTimer? _topmostTimer;
    private DispatcherTimer? _mouseListener;

    // P/Invoke for global mouse tracking
    private IntPtr _hookId = IntPtr.Zero;
    private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
    private LowLevelMouseProc? _mouseProc;

    private const int WH_MOUSE_LL = 14;
    private const int WM_LBUTTONDOWN = 0x0201;

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);

    [StructLayout(LayoutKind.Sequential)]
    private struct MSLLHOOKSTRUCT
    {
        public int x;
        public int y;
    }

    public MainWindow()
    {
        InitializeComponent();

        // IMPORTANT: keep on top so it never disappears behind taskbar
        Topmost = true;
        ShowInTaskbar = false;
        ShowActivated = true;

        _settings = _settingsManager.Load();

        _trayIcon.ExitRequested += OnExitRequested;
        _trayIcon.ExitRequested += () => System.Windows.Application.Current.Shutdown();

        _screenHelper.DisplaySettingsChanged += OnDisplaySettingsChanged;

        SourceInitialized += OnSourceInitialized;
        Loaded += OnLoaded;
        Closed += OnClosed;

        SetupFullscreenDetection();
        SetupTopmostTimer();
        SetupMouseListener();
    }

    private void OnExitRequested()
    {
        _settingsManager.Save(_settings);
    }

    private void ForceTopMost()
    {
        var hwnd = new WindowInteropHelper(this).Handle;
    
        NativeMethods.SetWindowPos(
            hwnd,
            new IntPtr(-1),
            0,
            0,
            0,
            0,
            0x0001 | 0x0002 | 0x0010
        );
    }

    private void OnSourceInitialized(object? sender, EventArgs e)
    {
        var hwnd = new WindowInteropHelper(this).Handle;

        // click-through behavior
        NativeMethods.MakeClickThrough(hwnd);

        // ensure layering is stable
        Topmost = true;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        PositionWindowOnMonitor();

        try
        {
            InitializeEngine();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show(
                $"Pocket Pal failed to load its sprites:\n\n{ex.Message}",
                "Pocket Pal - Asset Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            System.Windows.Application.Current.Shutdown();
            return;
        }

        // force visible spawn position
        if (_engine != null)
        {
            _engine.Movement.Position = new Models.Vector2D(
                PetCanvas.Width / 2,
                _groundY
            );
        }

        _loop = new GameLoop(delta => _engine!.Update(delta));
        _loop.Start();
    }

    private void InitializeEngine()
    {
        var loader = new SpriteAssetLoader();
        var clips = loader.LoadAll();
        var library = new AnimationLibrary(clips);

        var idleClip = library.Get(Models.AnimationKey.Idle);

        double spriteHeight = idleClip.FrameHeight * _spriteScale;

        // FIXED: correct screen-based grounding
        var workArea = SystemParameters.WorkArea;

        _groundY = (workArea.Bottom - Top) - spriteHeight;

        var movement = new MovementController
        {
            AreaWidth = PetCanvas.Width > 0 ? PetCanvas.Width : Width,
            GroundY = _groundY,
            SpriteWidth = idleClip.FrameWidth * _spriteScale
        };

        movement.Position = new Models.Vector2D(
            movement.AreaWidth / 2,
            _groundY);

        var renderer = new PetRenderer(PetImage, PetCanvas, _spriteScale);

        _engine = new PetEngine(
            movement,
            library,
            renderer,
            _settings.AnimationFramesPerSecond,
            new Random());
    }

    private void PositionWindowOnMonitor()
    {
        var workArea = _screenHelper.GetWorkArea(_settings.PreferredMonitorIndex);

        const double windowHeight = 260;

        Left = workArea.Left;
        Top = workArea.Bottom - windowHeight;
        Width = workArea.Width;
        Height = windowHeight;

        PetCanvas.Width = Width;
        PetCanvas.Height = Height;
    }

    // CLICK TO MOVE
    protected override void OnMouseDown(MouseButtonEventArgs e)
    {
        base.OnMouseDown(e);
    
        if (_engine is null)
            return;
    
    
        var pos = e.GetPosition(this);
    
    
        _engine.Movement.SetTarget(pos.X);
    }

    // Keep window always on top
    private void SetupTopmostTimer()
    {
        _topmostTimer = new DispatcherTimer();
        _topmostTimer.Interval = TimeSpan.FromMilliseconds(100);
        _topmostTimer.Tick += (_, __) =>
        {
            if (!Topmost)
            {
                Topmost = true;
            }
        };
        _topmostTimer.Start();
    }

    // Listen for global mouse clicks
    private void SetupMouseListener()
    {
        _mouseProc = MouseHookCallback;
        using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
        using (var curModule = curProcess.MainModule)
        {
            if (curModule != null)
            {
                _hookId = SetWindowsHookEx(WH_MOUSE_LL, _mouseProc, GetModuleHandle(curModule.ModuleName), 0);
            }
        }
    }

    private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam == (IntPtr)WM_LBUTTONDOWN)
        {
            var hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
            
            // Convert screen coordinates to pet window coordinates
            var screenPoint = new System.Windows.Point(hookStruct.x, hookStruct.y);
            var windowPoint = PointFromScreen(screenPoint);
            
            // Check if click is within our window bounds
            var screenHeight = SystemParameters.PrimaryScreenHeight;

            // Only bottom 40 pixels = taskbar zone
            bool clickedTaskbarArea =
                hookStruct.y >= screenHeight - 40;
            
            if (clickedTaskbarArea)
            {
                // Move pet to click location
                if (_engine is not null)
                {
                    Dispatcher.Invoke(() =>
                    {
                        _engine.Movement.SetTarget(windowPoint.X);
                    });
                }
            }
        }

        return CallNextHookEx(_hookId, nCode, wParam, lParam);
    }

    // FULLSCREEN DETECTION
    private void SetupFullscreenDetection()
    {
        _fullscreenTimer = new DispatcherTimer();
        _fullscreenTimer.Interval = TimeSpan.FromMilliseconds(500);

        _fullscreenTimer.Tick += (_, __) =>
        {
            var workArea = SystemParameters.WorkArea;
            var screenHeight = SystemParameters.PrimaryScreenHeight;

            if (Math.Abs(workArea.Height - screenHeight) < 2)
            {
                Show();
            }
            else
            {
                Hide();
            }
        };

        _fullscreenTimer.Start();
    }

    private void OnDisplaySettingsChanged()
    {
        Dispatcher.Invoke(() =>
        {
            PositionWindowOnMonitor();

            if (_engine is not null)
            {
                _engine.Movement.AreaWidth = PetCanvas.Width;

                var pos = _engine.Movement.Position;
                double maxX = Math.Max(0, _engine.Movement.AreaWidth - _engine.Movement.SpriteWidth);
                pos.X = Math.Min(pos.X, maxX);
                _engine.Movement.Position = pos;
            }
        });
    }

    private void OnClosed(object? sender, EventArgs e)
    {
        _loop?.Stop();
        _screenHelper.Dispose();
        _trayIcon.Dispose();
        _fullscreenTimer?.Stop();
        _topmostTimer?.Stop();
        
        if (_hookId != IntPtr.Zero)
        {
            UnhookWindowsHookEx(_hookId);
        }
    }
}
