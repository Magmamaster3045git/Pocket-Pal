namespace PocketPal.Settings;

/// <summary>
/// Plain data object for user-configurable / persisted settings. Kept as a
/// simple POCO (no logic) so it serializes cleanly to JSON and can grow
/// with future features (hunger decay rate, sound volume, chosen pet
/// skin, etc.) without touching unrelated systems.
/// </summary>
public sealed class AppSettings
{
    public int AnimationFramesPerSecond { get; set; } = 8;

    public double WalkSpeed { get; set; } = 40;
    public double RunSpeed { get; set; } = 110;

    public double SpriteScale { get; set; } = 2.0; // pixel-art upscale factor

    /// <summary>Index into Screen.AllScreens the pet should live on. -1 = primary monitor.</summary>
    public int PreferredMonitorIndex { get; set; } = -1;

    public bool StartMinimizedToTray { get; set; } = false;

    // --- Reserved for future systems (kept here now so the schema is stable) ---
    public int Hunger { get; set; } = 100;
    public int Happiness { get; set; } = 100;
    public bool SoundEnabled { get; set; } = true;

    public bool StartWithWindows { get; set; }
    
    public bool ClickThrough { get; set; } = true;
    
    public bool StaticMode { get; set; }
    
    public string SelectedCharacter { get; set; } = "Dog";
}
