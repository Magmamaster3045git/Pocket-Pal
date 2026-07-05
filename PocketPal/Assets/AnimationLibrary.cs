using PocketPal.Animation;
using PocketPal.Models;

namespace PocketPal.Assets;

/// <summary>
/// Central lookup for animation clips. Keeps the "which animation goes with
/// which state" mapping in one place instead of scattering it through the
/// state machine, so new states/animations are a one-line change here.
/// </summary>
public sealed class AnimationLibrary
{
    private readonly IReadOnlyDictionary<AnimationKey, AnimationClip> _clips;

    public AnimationLibrary(IReadOnlyDictionary<AnimationKey, AnimationClip> clips)
    {
        _clips = clips;
    }

    public AnimationClip Get(AnimationKey key) => _clips[key];

    /// <summary>
    /// Resolves the correct clip for a state + facing direction, handling
    /// the states that have left/right variants (Walking, Running) and
    /// the ones that don't.
    /// </summary>
    public AnimationClip Resolve(PetStateType state, Direction direction)
    {
        AnimationKey key = state switch
        {
            PetStateType.Idle => AnimationKey.Idle,
            PetStateType.Walking => direction == Direction.Left ? AnimationKey.Walk_Left : AnimationKey.Walk_Right,
            PetStateType.Running => direction == Direction.Left ? AnimationKey.Run_Left : AnimationKey.Run_Right,
            PetStateType.Sitting => AnimationKey.Sit,
            PetStateType.Sleeping => AnimationKey.Sleep,
            _ => AnimationKey.Idle
        };

        return Get(key);
    }
}
