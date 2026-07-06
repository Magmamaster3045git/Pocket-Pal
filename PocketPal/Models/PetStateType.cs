namespace PocketPal.Models;

/// <summary>
/// The finite set of high-level behavioral states the pet can be in.
/// Only one is ever active at a time (enforced by PetStateMachine).
/// </summary>
public enum PetStateType
{
    Idle,
    Walking,
    Running,
    Sitting,
    Sleeping
}
