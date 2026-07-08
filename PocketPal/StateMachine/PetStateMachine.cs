using PocketPal.Models;
using PocketPal.StateMachine.States;

namespace PocketPal.StateMachine;

/// <summary>
/// Owns exactly one active IPetState at a time and handles Enter/Exit
/// lifecycle calls on transitions. This is the only class allowed to
/// swap the current state.
/// </summary>
public sealed class PetStateMachine
{
    public PetContext Context { get; }

    public IPetState CurrentState { get; private set; }

    public event Action<IPetState>? StateChanged;


    public PetStateMachine(PetContext context, IPetState initialState)
    {
        Context = context;

        CurrentState = initialState;
        CurrentState.Enter(Context);
    }


    public PetStateType CurrentType => CurrentState.Type;


    public void Update(double deltaSeconds)
    {
        Context.TimeInState += deltaSeconds;

        IPetState? next = CurrentState.Update(
            Context,
            deltaSeconds
        );

        if (next is not null)
            TransitionTo(next);
    }


    private void TransitionTo(IPetState next)
    {
        CurrentState.Exit(Context);

        CurrentState = next;

        Context.TimeInState = 0;

        CurrentState.Enter(Context);

        StateChanged?.Invoke(CurrentState);
    }


    /// <summary>
    /// Force a transition, such as clicking the pet.
    /// </summary>
    public void ForceTransition(IPetState next)
    {
        TransitionTo(next);
    }
}
