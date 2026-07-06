namespace PocketPal.Models;

/// <summary>
/// Identifies a specific animation clip. Each value corresponds 1:1 with a
/// folder name under Assets/Sprites/. Adding a new animation later only
/// requires adding a value here + a folder; no other code changes needed
/// as long as the folder-naming convention is followed by AssetLoader.
/// </summary>
public enum AnimationKey
{
    Idle,
    Walk_Left,
    Walk_Right,
    Run_Left,
    Run_Right,
    Sit,
    Sleep
}
