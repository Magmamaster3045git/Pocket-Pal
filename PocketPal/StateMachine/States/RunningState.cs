using PocketPal.Models;

namespace PocketPal.StateMachine.States;

public sealed class RunningState : IPetState
{
    public PetStateType Type => PetStateType.Running;

    public const double SpeedPixelsPerSecond = 110;

    private double _duration;


    public void Enter(PetContext context)
    {
        _duration = 1.0 + context.Random.NextDouble() * 2.5;
    }


    public IPetState? Update(
        PetContext context,
        double deltaSeconds)
    {
        // Running caused by taskbar click.
        // This is allowed even in Static Mode.
        if (context.Movement.HasTarget)
        {
            context.Movement.MoveTowardsTarget(
                SpeedPixelsPerSecond,
                deltaSeconds);


            // Reached destination
            if (!context.Movement.HasTarget)
            {
                context.ForceSit = true;


                // Static Mode:
                // immediately return to resting cycle forever
                if (context.StaticMode)
                {
                    return context.Random.Next(2) == 0
                        ? new SittingState()
                        : new SleepingState();
                }


                // Normal mode:
                // sit after reaching destination
                return new SittingState(true);
            }


            return null;
        }


        // Static Mode:
        // never allow random running
        if (context.StaticMode)
        {
            return context.Random.Next(2) == 0
                ? new SittingState()
                : new SleepingState();
        }


        // Normal random running
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
