using System.Windows;
using System.Windows.Interop;
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
    private double _spriteScale;

    public MainWindow()
    {
        InitializeComponent();

        _settings = _settingsManager.Load();
        _spriteScale = _settings.SpriteScale;

        _trayIcon.ExitRequested += OnExitRequested;
        _trayIcon.ExitRequested += () => Application.Current.Shutdown();

        _screenHelper.DisplaySettingsChanged += OnDisplaySettingsChanged;

        SourceInitialized += OnSourceInitialized;
        Loaded += OnLoaded;
        Closed += OnClosed;
    }

    private void OnExitRequested()
    {
        // Placeholder for future graceful-shutdown hooks (save pet state, etc.)
        _settingsManager.Save(_settings);
    }

    private void OnSourceInitialized(object? sender, EventArgs e)
    {
        // Must happen after the Win32 window handle exists.
        var hwnd = new WindowInteropHelper(this).Handle;
        NativeMethods.MakeClickThrough(hwnd);
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
            // Fail loudly but don't crash silently - a broken asset folder
            // should be obvious to whoever is developing/deploying this.
            MessageBox.Show(
                $"Pocket Pal failed to load its sprites:\n\n{ex.Message}",
                "Pocket Pal - Asset Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Application.Current.Shutdown();
            return;
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

        var movement = new MovementController
        {
            AreaWidth = PetCanvas.Width > 0 ? PetCanvas.Width : Width,
            // Ground line = bottom of the window (which sits flush with the
            // monitor's work area bottom), minus the sprite's own height,
            // since Position tracks the sprite's top-left corner.
            GroundY = PetCanvas.Height - spriteHeight,
            SpriteWidth = idleClip.FrameWidth * _spriteScale
        };
        movement.Position = new Models.Vector2D(movement.AreaWidth / 2, movement.GroundY);

        var renderer = new PetRenderer(PetImage, PetCanvas, _spriteScale);

        var random = new Random();
        _engine = new PetEngine(movement, library, renderer, _settings.AnimationFramesPerSecond, random);
    }

    private void PositionWindowOnMonitor()
    {
        var workArea = _screenHelper.GetWorkArea(_settings.PreferredMonitorIndex);

        // The window spans the full width of the monitor's work area and
        // is tall enough to comfortably fit jump height + sprite; it sits
        // flush with the bottom of the work area (i.e. just above the taskbar).
        const double windowHeight = 260;

        Left = workArea.Left;
        Top = workArea.Bottom - windowHeight;
        Width = workArea.Width;
        Height = windowHeight;

        PetCanvas.Width = Width;
        PetCanvas.Height = Height;
    }

    private void OnDisplaySettingsChanged()
    {
        // Marshal back to the UI thread - SystemEvents fires off-thread.
        Dispatcher.Invoke(() =>
        {
            PositionWindowOnMonitor();

            if (_engine is not null)
            {
                _engine.Movement.AreaWidth = PetCanvas.Width;

                // Re-clamp position so the pet isn't left stranded off-screen
                // after a resolution shrink.
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
    }
}
