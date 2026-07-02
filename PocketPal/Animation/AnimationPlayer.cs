using System.Windows.Media.Imaging;

namespace PocketPal.Animation;

/// <summary>
/// Advances animation frames at a FIXED rate (default 8 FPS) using time
/// accumulation, so the retro pixel-art cadence stays constant no matter
/// how often Update() is called by the game loop (movement can still be
/// updated at 60 FPS for smoothness - see GameLoop).
/// </summary>
public sealed class AnimationPlayer
{
    private readonly double _secondsPerFrame;
    private double _accumulator;
    private int _frameIndex;
    private AnimationClip? _clip;

    public event Action? FrameChanged;
    public event Action? AnimationCompleted; // fires once for non-looping clips

    public AnimationPlayer(int framesPerSecond = 8)
    {
        if (framesPerSecond <= 0) throw new ArgumentOutOfRangeException(nameof(framesPerSecond));
        _secondsPerFrame = 1.0 / framesPerSecond;
    }

    public AnimationClip? CurrentClip => _clip;
    public BitmapImage? CurrentFrame => _clip is null ? null : _clip.Frames[_frameIndex];
    public int CurrentFrameIndex => _frameIndex;

    /// <summary>
    /// Switch to a new clip. Resets playback to frame 0 unless the same
    /// clip is already playing (avoids visual "pop" on redundant sets).
    /// </summary>
    public void Play(AnimationClip clip)
    {
        if (ReferenceEquals(_clip, clip)) return;

        _clip = clip;
        _frameIndex = 0;
        _accumulator = 0;
        FrameChanged?.Invoke();
    }

    /// <summary>Advance playback by elapsed real time (seconds).</summary>
    public void Update(double deltaSeconds)
    {
        if (_clip is null || _clip.FrameCount <= 1) return;

        _accumulator += deltaSeconds;

        while (_accumulator >= _secondsPerFrame)
        {
            _accumulator -= _secondsPerFrame;
            AdvanceFrame();
        }
    }

    private void AdvanceFrame()
    {
        if (_clip is null) return;

        int next = _frameIndex + 1;

        if (next >= _clip.FrameCount)
        {
            if (_clip.Loop)
            {
                _frameIndex = 0;
                FrameChanged?.Invoke();
            }
            else
            {
                // Hold on the last frame and notify completion once.
                _frameIndex = _clip.FrameCount - 1;
                AnimationCompleted?.Invoke();
            }
            return;
        }

        _frameIndex = next;
        FrameChanged?.Invoke();
    }
}
