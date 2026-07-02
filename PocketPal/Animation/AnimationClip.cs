using System.Windows.Media.Imaging;
using PocketPal.Models;

namespace PocketPal.Animation;

/// <summary>
/// An ordered sequence of frames for a single animation (e.g. "Walk_Left").
/// Frame images are pre-decoded BitmapImages so playback never touches disk.
/// Supports any frame count - callers never need to know how many frames exist.
/// </summary>
public sealed class AnimationClip
{
    public AnimationKey Key { get; }
    public IReadOnlyList<BitmapImage> Frames { get; }
    public bool Loop { get; }

    /// <summary>Natural pixel size of a frame, used to size the render surface.</summary>
    public int FrameWidth { get; }
    public int FrameHeight { get; }

    public int FrameCount => Frames.Count;

    public AnimationClip(AnimationKey key, IReadOnlyList<BitmapImage> frames, bool loop = true)
    {
        if (frames.Count == 0)
            throw new InvalidOperationException($"Animation clip '{key}' has no frames. Check Assets/Sprites/{key}.");

        Key = key;
        Frames = frames;
        Loop = loop;
        FrameWidth = frames[0].PixelWidth;
        FrameHeight = frames[0].PixelHeight;
    }
}
