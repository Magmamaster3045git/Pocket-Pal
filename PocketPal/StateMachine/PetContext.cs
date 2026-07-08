using PocketPal.Assets;
using PocketPal.Models;
using PocketPal.Movement;

namespace PocketPal.StateMachine;

/// <summary>
/// Everything a state needs to read/modify, gathered in one place so
/// states don't need direct references to the renderer, window, or engine.
/// </summary>
public sealed class PetContext
{
    public MovementController Movement { get; }
    public AnimationLibrary Animations { get; }
    public Random Random { get; }

    /// <summary>Seconds the current state has been active.</summary>
    public double TimeInState { get; internal set; }

    /// <summary>
    /// True when the user has clicked the pet and wants it to sit.
    /// Cleared when the sit ends or the user clicks the taskbar.
    /// </summary>
    public bool ForceSit { get; set; }

    public Direction FacingDirection
    {
        get => Movement.Direction;
        set => Movement.Direction = value;
    }

    public PetContext(
        MovementController movement,
        AnimationLibrary animations,
        Random random)
    {
        Movement = movement;
        Animations = animations;
        Random = random;
    }
}
