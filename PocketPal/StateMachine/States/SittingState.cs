using PocketPal.Models;

namespace PocketPal.StateMachine.States;

/// <summary>Pet sits for a short while, then resumes normal ground behavior.</summary>
public sealed class SittingState : IPetState
{
    public PetStateType Type => PetStateType.Sitting;

    private double _duration;

    public void Enter(PetContext context)
    {
        _duration = 3.0 + context.Random.NextDouble() * 3.0;
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
