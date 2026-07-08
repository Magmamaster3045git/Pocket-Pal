using PocketPal.Models;

namespace PocketPal.StateMachine.States;

/// <summary>
/// Pet sits until 10 seconds have passed or the user clicks the taskbar.
/// </summary>
public sealed class SittingState : IPetState
{
    public PetStateType Type => PetStateType.Sitting;

    private const double SitDuration = 10.0;

    public void Enter(PetContext context)
    {
        // Stop moving while sitting
        context.Movement.Velocity = new Vector2D(0, 0);
    }

    public IPetState? Update(PetContext context, double deltaSeconds)
    {
        // If the user clicked somewhere on the taskbar,
        // immediately get up and run there.
        if (context.Movement.HasTarget)
        {
            context.ForceSit = false;
            return new RunningState();
        }

        // Sit for 10 seconds
        if (context.TimeInState >= SitDuration)
        {
            context.ForceSit = false;
            return PetBehaviorPicker.PickNextGroundState(context);
        }

        return null;
    }

    public void Exit(PetContext context)
    {
    }
}
