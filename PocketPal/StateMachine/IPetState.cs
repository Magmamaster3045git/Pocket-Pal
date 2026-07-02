using PocketPal.Models;

namespace PocketPal.StateMachine;

/// <summary>
/// Contract for a single pet behavior state. Each state owns its own
/// animation choice, movement speed, direction changes and duration/exit
/// condition - the machine itself stays dumb and just hosts whichever
/// state is active.
/// </summary>
public interface IPetState
{
    PetStateType Type { get; }

    /// <summary>Called once when the state becomes active.</summary>
    void Enter(PetContext context);

    /// <summary>
    /// Called every game-loop tick. Return the next state to transition
    /// into, or null to keep running this state.
    /// </summary>
    IPetState? Update(PetContext context, double deltaSeconds);

    /// <summary>Called once when the state is about to be replaced.</summary>
    void Exit(PetContext context);
}
