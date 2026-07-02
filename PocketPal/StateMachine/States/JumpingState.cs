using PocketPal.Models;
using PocketPal.StateMachine;

namespace PocketPal.StateMachine.States;

/// <summary>
/// Pet leaps upward. Gravity pulls it back down.
/// </summary>
public sealed class JumpingState : IPetState
{
    public PetStateType Type => PetStateType.Jumping;

    private const double InitialJumpSpeed = 380;

    public void Enter(PetContext context)
    {
        context.Movement.StartJump(InitialJumpSpeed);
    }

    public IPetState? Update(PetContext context, double deltaSeconds)
    {
        context.Movement.ApplyGravity(deltaSeconds);

        if (context.Movement.Velocity.Y > 0)
            return new FallingState();

        return null;
    }

    public void Exit(PetContext context)
    {
    }
}
