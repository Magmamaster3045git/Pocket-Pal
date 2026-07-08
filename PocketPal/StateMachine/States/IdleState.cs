using PocketPal.Models;

namespace PocketPal.StateMachine.States;

/// <summary>
/// Pet stands still, plays the Idle animation, then decides what to do next.
/// In Static Mode the pet immediately switches into a resting state instead
/// of wandering around.
/// </summary>
public sealed class IdleState : IPetState
{
    public PetStateType Type => PetStateType.Idle;

    private double _duration;


    public void Enter(PetContext context)
    {
        if (context.StaticMode)
        {
            _duration = 0;
        }
        else
        {
            // Idle for somewhere between 2 and 6 seconds.
            _duration = 2.0 + context.Random.NextDouble() * 4.0;
        }


        context.Movement.Velocity =
            new Vector2D(
                0,
                context.Movement.Velocity.Y);
    }


    public IPetState? Update(
        PetContext context,
        double deltaSeconds)
    {
        // User forced sit
        if (context.ForceSit)
            return new SittingState(true);


        // Static Mode:
        // Only allow sitting or sleeping.
        if (context.StaticMode)
        {
            return context.Random.Next(2) == 0
                ? new SittingState()
                : new SleepingState();
        }


        // Normal idle timer
        if (context.TimeInState < _duration)
            return null;


        return PetBehaviorPicker.PickNextGroundState(context);
    }


    public void Exit(PetContext context)
    {
    }
}
