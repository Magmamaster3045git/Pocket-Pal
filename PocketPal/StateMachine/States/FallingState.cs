namespace PocketPal.StateMachine.States;

/// <summary>Pet descends under gravity until it lands, then returns to normal ground behavior.</summary>
public sealed class FallingState : IPetState
{
    public PetStateType Type => PetStateType.Falling;

    public void Enter(PetContext context) { }

    public IPetState? Update(PetContext context, double deltaSeconds)
    {
        context.Movement.ApplyGravity(deltaSeconds);

        if (context.Movement.IsGrounded)
            return PetBehaviorPicker.PickNextGroundState(context);

        return null;
    }

    public void Exit(PetContext context) { }
}
