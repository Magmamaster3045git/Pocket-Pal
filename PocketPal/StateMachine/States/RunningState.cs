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
        // Running to a mouse/taskbar click location
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
                // return to permanent resting behaviour
                if (context.StaticMode)
                {
                    return context.Random.Next(2) == 0
                        ? new SittingState()
                        : new SleepingState();
                }


                // Normal mode:
                // sit for 10 seconds after being clicked to move
                return new SittingState(true);
            }


            return null;
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
