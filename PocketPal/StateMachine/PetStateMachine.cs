using PocketPal.Models;
using PocketPal.StateMachine.States;

namespace PocketPal.StateMachine;

/// <summary>
/// Owns exactly one active IPetState at a time and handles Enter/Exit
/// lifecycle calls on transitions. This is the only class allowed to
/// swap the current state - everything else just observes CurrentState.
/// </summary>
public sealed class PetStateMachine
{
    private readonly PetContext _context;

    public IPetState CurrentState { get; private set; }
    public event Action<IPetState>? StateChanged;

    public PetStateMachine(PetContext context, IPetState initialState)
    {
        _context = context;
        CurrentState = initialState;
        CurrentState.Enter(_context);
    }

    public PetStateType CurrentType => CurrentState.Type;

    public void Update(double deltaSeconds)
    {
        _context.TimeInState += deltaSeconds;

        IPetState? next = CurrentState.Update(_context, deltaSeconds);

        if (next is not null)
            TransitionTo(next);
    }

    private void TransitionTo(IPetState next)
    {
        CurrentState.Exit(_context);
        CurrentState = next;
        _context.TimeInState = 0;
        CurrentState.Enter(_context);
        StateChanged?.Invoke(CurrentState);
    }

    /// <summary>Force a transition (e.g. from a future "click to make pet jump" interaction).</summary>
    public void ForceTransition(IPetState next) => TransitionTo(next);
}
