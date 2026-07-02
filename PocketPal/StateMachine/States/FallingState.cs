using PocketPal.Models;
using PocketPal.StateMachine;

namespace PocketPal.StateMachine.States;

/// <summary>
/// Pet falls until it lands.
/// </summary>
public sealed class FallingState : IPetState
{
    public PetStateType Type => PetStateType.Falling;

    public void Enter(PetContext context)
    {
    }

    public IPetState? Update(PetContext context, double deltaSeconds)
    {
        context.Movement.ApplyGravity(deltaSeconds);

        if (context.Movement.IsGrounded)
            return PetBehaviorPicker.PickNextGroundState(context);

        return null;
    }

    public void Exit(PetContext context)
    {
    }
}
