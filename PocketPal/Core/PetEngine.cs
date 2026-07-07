using PocketPal.Animation;
using PocketPal.Assets;
using PocketPal.Models;
using PocketPal.Movement;
using PocketPal.Rendering;
using PocketPal.StateMachine;
using PocketPal.StateMachine.States;

namespace PocketPal.Core;

/// <summary>
/// The pet's runtime brain. Each tick it: updates the state machine,
/// resolves the animation the current state+direction should be playing,
/// advances animation playback, and hands the result to the renderer.
///
/// This class intentionally only orchestrates - the actual behavior lives
/// in the state classes, actual movement math lives in MovementController,
/// actual animation timing lives in AnimationPlayer. Keeping this thin is
/// what keeps the project easy to expand later (hunger/happiness systems
/// can hook into Update() without touching state or animation code).
/// </summary>
public sealed class PetEngine
{
    public MovementController Movement { get; }
    public AnimationLibrary Animations { get; }
    public AnimationPlayer Player { get; }
    public PetStateMachine States { get; }
    private readonly PetRenderer _renderer;

    public PetEngine(
        MovementController movement,
        AnimationLibrary animations,
        PetRenderer renderer,
        int framesPerSecond,
        Random random)
    {
        Movement = movement;
        Animations = animations;
        _renderer = renderer;
        Player = new AnimationPlayer(framesPerSecond);

        var context = new PetContext(movement, animations, random);
        States = new PetStateMachine(context, new IdleState());
    }

    public void Update(double deltaSeconds)
    {
        // NEW: If we have a destination, walk toward it.
        if (Movement.HasTarget)
        {
            Movement.MoveTowardsTarget(220, deltaSeconds);
        }

        States.Update(deltaSeconds);

        AnimationClip clip = Animations.Resolve(
            States.CurrentType,
            Movement.Direction);

        Player.Play(clip);
        Player.Update(deltaSeconds);

        _renderer.DrawFrame(Player);
        _renderer.PositionSprite(Movement.Position);
    }
}
