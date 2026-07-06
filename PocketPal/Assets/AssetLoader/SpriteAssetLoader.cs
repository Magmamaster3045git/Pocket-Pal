using System.IO;
using System.Windows.Media.Imaging;
using PocketPal.Animation;
using PocketPal.Models;

namespace PocketPal.Assets.AssetLoader;

/// <summary>
/// Loads sprite-sheet frames from disk into AnimationClips.
///
/// Convention over configuration: each AnimationKey maps to a folder of the
/// same name under Assets/Sprites/, containing sequentially-named PNG
/// frames (frame_000.png, frame_001.png, ...). Dropping new/more frames
/// into a folder is picked up automatically - no code or recompilation
/// needed to add art, only to add a brand-new AnimationKey.
/// </summary>
public sealed class SpriteAssetLoader
{
    private const string SpritesFolderName = "Sprites";

    private readonly string _rootPath;

    /// <param name="rootPath">
    /// Base "Assets" directory. Defaults to the folder next to the executable.
    /// </param>
    public SpriteAssetLoader(string? rootPath = null)
    {
        _rootPath = rootPath ?? Path.Combine(AppContext.BaseDirectory, "Assets");
    }

    /// <summary>
    /// Loads every AnimationKey defined in the enum. Throws with a clear,
    /// actionable message if a required folder is missing or empty, so
    /// asset problems fail fast at startup instead of showing a blank pet.
    /// </summary>
    public IReadOnlyDictionary<AnimationKey, AnimationClip> LoadAll()
    {
        var result = new Dictionary<AnimationKey, AnimationClip>();

        foreach (AnimationKey key in Enum.GetValues<AnimationKey>())
        {
            result[key] = LoadClip(key);
        }

        return result;
    }

    public AnimationClip LoadClip(AnimationKey key)
    {
        string folder = Path.Combine(_rootPath, SpritesFolderName, key.ToString());

        if (!Directory.Exists(folder))
        {
            throw new DirectoryNotFoundException(
                $"Missing sprite folder for animation '{key}'. Expected: {folder}\n" +
                "Create the folder and add frame_000.png, frame_001.png, ... (any PNG count).");
        }

        var files = Directory.GetFiles(folder, "*.png")
            .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (files.Count == 0)
        {
            throw new InvalidOperationException(
                $"Sprite folder for animation '{key}' contains no PNG frames: {folder}");
        }

        var frames = files.Select(LoadFrame).ToList();

        // Sleep/Sit hold on the final pose rather than looping back to frame 0
        // by default in most desktop-pet art; adjust here if your art differs.
        bool loop = true; // All remaining animations loop

        return new AnimationClip(key, frames, loop);
    }

    private static BitmapImage LoadFrame(string path)
    {
        var bitmap = new BitmapImage();
        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad; // decode immediately, release file handle
        bitmap.UriSource = new Uri(path, UriKind.Absolute);
        bitmap.EndInit();
        bitmap.Freeze(); // makes it cross-thread safe and slightly cheaper to render
        return bitmap;
    }
}
