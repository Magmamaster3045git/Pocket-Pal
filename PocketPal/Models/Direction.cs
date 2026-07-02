namespace PocketPal.Models;

/// <summary>
/// Horizontal facing direction of the pet. Kept simple and separate from
/// PetStateType so movement code and animation-selection code stay decoupled.
/// </summary>
public enum Direction
{
    Left,
    Right
}
