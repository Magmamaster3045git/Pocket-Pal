using PocketPal.Models;

namespace PocketPal.StateMachine.States;

public sealed class SittingState : IPetState
{
    public PetStateType Type => PetStateType.Sitting;

    private double _duration;

    private readonly bool _forced;


    public SittingState(bool forced = false)
    {
        _forced = forced;
    }


    public void Enter(PetContext context)
    {
        // Static mode:
        // stay sitting until another static decision is made
        if (context.StaticMode && !_forced)
        {
            _duration = 8.0 + context.Random.NextDouble() * 12.0;
        }
        else
        {
            // Click destination sit = 10 seconds
            // Normal random sit = 3-6 seconds
            _duration = _forced
                ? 10.0
                : 3.0 + context.Random.NextDouble() * 3.0;
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
        if (context.TimeInState < _duration)
            return null;


        // Static Mode only alternates between sitting and sleeping
        if (context.StaticMode && !_forced)
        {
            return context.Random.NextDouble() < 0.5
                ? new SittingState()
                : new SleepingState();
        }


        return PetBehaviorPicker.PickNextGroundState(context);
    }


    public void Exit(PetContext context)
    {
    }
}
