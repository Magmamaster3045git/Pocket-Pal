using PocketPal.Models;
using PocketPal.StateMachine.States;

namespace PocketPal.StateMachine;

/// <summary>
/// Owns exactly one active IPetState at a time and handles Enter/Exit
/// lifecycle calls on transitions.
/// </summary>
public sealed class PetStateMachine
{
    public PetContext Context { get; }

    public IPetState CurrentState { get; private set; }

    public event Action<IPetState>? StateChanged;


    public PetStateMachine(
        PetContext context,
        IPetState initialState)
    {
        Context = context;

        CurrentState = initialState;

        CurrentState.Enter(Context);
    }


    public PetStateType CurrentType =>
        CurrentState.Type;


    public void Update(double deltaSeconds)
    {
        Context.TimeInState += deltaSeconds;


        IPetState? next =
            CurrentState.Update(
                Context,
                deltaSeconds);


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
    /// Used for clicks and forced actions.
    /// Allows RunningState temporarily even in Static Mode.
    /// </summary>
    public void ForceTransition(IPetState next)
    {
        TransitionTo(next);
    }


    /// <summary>
    /// Returns the pet to static resting mode.
    /// </summary>
    public void ReturnToStaticRest()
    {
        if (!Context.StaticMode)
            return;


        TransitionTo(
            Context.Random.Next(2) == 0
                ? new SittingState()
                : new SleepingState());
    }
}
