using PocketPal.Models;
using PocketPal.StateMachine;

namespace PocketPal.StateMachine.States;

/// <summary>
/// Pet walks along the ground at a moderate speed for a random duration.
/// Direction flips automatically via MovementController when it hits a
/// screen edge.
///
/// In Static Mode walking is disabled and the pet returns to resting
/// behaviours only.
/// </summary>
public sealed class WalkingState : IPetState
{
    public PetStateType Type => PetStateType.Walking;

    public const double SpeedPixelsPerSecond = 40;

    private double _duration;


    public void Enter(PetContext context)
    {
        _duration = 2.0 + context.Random.NextDouble() * 5.0;
    }


    public IPetState? Update(
        PetContext context,
        double deltaSeconds)
    {
        // Static Mode:
        // no wandering allowed
        if (context.StaticMode)
        {
            return context.Random.Next(2) == 0
                ? new SittingState()
                : new SleepingState();
        }


        context.Movement.MoveHorizontal(
            SpeedPixelsPerSecond,
            deltaSeconds);


        if (context.TimeInState >= _duration)
            return PetBehaviorPicker.PickNextGroundState(context);


        return null;
    }


    public void Exit(PetContext context)
    {
    }
}
