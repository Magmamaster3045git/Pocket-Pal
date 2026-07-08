using PocketPal.Models;

namespace PocketPal.StateMachine.States;

/// <summary>
/// Pet stands still, plays the Idle animation, then after a random
/// duration randomly decides to walk, run, sit, or idle again.
/// </summary>
public sealed class IdleState : IPetState
{
    public PetStateType Type => PetStateType.Idle;

    private double _duration;

    public void Enter(PetContext context)
    {
        // Idle for somewhere between 2 and 6 seconds before deciding what to do next.
        _duration = 2.0 + context.Random.NextDouble() * 4.0;

        context.Movement.Velocity = new Vector2D(
            0,
            context.Movement.Velocity.Y
        );
    }

    public IPetState? Update(PetContext context, double deltaSeconds)
    {
        // User clicked the pet
        if (context.ForceSit)
            return new SittingState();

        // Stay idle until the timer expires
        if (context.TimeInState < _duration)
            return null;

        // Resume normal AI
        return PetBehaviorPicker.PickNextGroundState(context);
    }

    public void Exit(PetContext context)
    {
    }
}
