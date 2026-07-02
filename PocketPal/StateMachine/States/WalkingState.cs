namespace PocketPal.StateMachine.States;

/// <summary>
/// Pet walks along the ground at a moderate speed for a random duration.
/// Direction flips automatically via MovementController when it hits a
/// screen edge - the state doesn't need to know or care.
/// </summary>
public sealed class WalkingState : IPetState
{
    public PetStateType Type => PetStateType.Walking;

    public const double SpeedPixelsPerSecond = 40;

    private double _duration;

    public void Enter(PetContext context)
    {
        _duration = 2.0 + context.Random.NextDouble() * 5.0;
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
