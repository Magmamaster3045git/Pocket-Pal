using PocketPal.Models;

namespace PocketPal.StateMachine.States;

/// <summary>
/// Pet sleeps for an extended duration before waking up.
/// In Static Mode it only alternates between sleeping and sitting.
/// </summary>
public sealed class SleepingState : IPetState
{
    public PetStateType Type => PetStateType.Sleeping;

    private double _duration;


    public void Enter(PetContext context)
    {
        _duration = 8.0 + context.Random.NextDouble() * 12.0;

        context.Movement.Velocity =
            new Vector2D(
                0,
                context.Movement.Velocity.Y);
    }


    public IPetState? Update(
        PetContext context,
        double deltaSeconds)
    {
        if (context.TimeInState < _duration)
            return null;


        // Static Mode:
        // never walk/run/idle, only swap between sleep and sit
        if (context.StaticMode)
        {
            return context.Random.NextDouble() < 0.5
                ? new SleepingState()
                : new SittingState();
        }


        // Normal AI behaviour
        return PetBehaviorPicker.PickNextGroundState(context);
    }


    public void Exit(PetContext context)
    {
    }
}
