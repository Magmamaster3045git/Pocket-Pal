namespace PocketPal.StateMachine.States;

/// <summary>
/// Pet leaps upward. Gravity (in MovementController) pulls it back down.
/// Once it starts descending we hand off to FallingState so a distinct
/// "Fall" animation can play, matching the required animation set.
/// </summary>
public sealed class JumpingState : IPetState
{
    public PetStateType Type => PetStateType.Jumping;

    private const double InitialJumpSpeed = 380; // pixels/sec upward

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

    public void Exit(PetContext context) { }
}
