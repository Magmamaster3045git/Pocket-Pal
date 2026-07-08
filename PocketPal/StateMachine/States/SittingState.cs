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
        // Static Mode:
        // sit as a resting pose, then randomly choose sit/sleep again
        if (context.StaticMode && !_forced)
        {
            _duration = 10.0 + context.Random.NextDouble() * 20.0;
        }
        else
        {
            // Click-to-move arrival:
            // sit for 10 seconds
            //
            // Normal AI:
            // sit for 3-6 seconds
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


        // Static Mode never wanders.
        // Only choose another resting pose.
        if (context.StaticMode && !_forced)
        {
            return context.Random.Next(2) == 0
                ? new SittingState()
                : new SleepingState();
        }


        // Normal behaviour resumes.
        return PetBehaviorPicker.PickNextGroundState(context);
    }


    public void Exit(PetContext context)
    {
    }
}
