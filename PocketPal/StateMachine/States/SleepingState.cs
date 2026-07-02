using PocketPal.Models;

namespace PocketPal.StateMachine.States;

/// <summary>Pet sleeps for an extended duration before waking up and picking a new behavior.</summary>
public sealed class SleepingState : IPetState
{
    public PetStateType Type => PetStateType.Sleeping;

    private double _duration;

    public void Enter(PetContext context)
    {
        _duration = 8.0 + context.Random.NextDouble() * 12.0;
        context.Movement.Velocity = new Vector2D(0, context.Movement.Velocity.Y);
    }

    public IPetState? Update(PetContext context, double deltaSeconds)
    {
        if (context.TimeInState < _duration)
            return null;

        return PetBehaviorPicker.PickNextGroundState(context);
    }

    public void Exit(PetContext context) { }
}
