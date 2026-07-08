using PocketPal.Assets;
using PocketPal.Models;
using PocketPal.Movement;

namespace PocketPal.StateMachine;

/// <summary>
/// Everything a state needs to read/modify, gathered in one place so
/// states don't need direct references to the renderer, window, or engine.
/// This is what keeps states testable and keeps the "no single God class"
/// design goal intact.
/// </summary>
public sealed class PetContext
{
    public MovementController Movement { get; }
    public AnimationLibrary Animations { get; }
    public Random Random { get; }

    /// <summary>Seconds the current state has been active. Reset on Enter by the state machine.</summary>
    public double TimeInState { get; internal set; }

    public Direction FacingDirection
    {
        get => Movement.Direction;
        set => Movement.Direction = value;
    }

    public bool ForceSit { get; set; }

    public PetContext(MovementController movement, AnimationLibrary animations, Random random)
    {
        Movement = movement;
        Animations = animations;
        Random = random;
    }
}
