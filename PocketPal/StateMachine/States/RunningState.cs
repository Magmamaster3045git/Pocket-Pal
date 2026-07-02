using PocketPal.Models;
using PocketPal.StateMachine;

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

    public IPetState? Update(PetContext context, double deltaSeconds)
    {
        context.Movement.MoveHorizontal(SpeedPixelsPerSecond, deltaSeconds);

        if (context.TimeInState >= _duration)
            return PetBehaviorPicker.PickNextGroundState(context);

        return null;
    }

    public void Exit(PetContext context) { }
}
