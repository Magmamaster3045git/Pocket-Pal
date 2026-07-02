namespace PocketPal.StateMachine.States;

/// <summary>
/// Centralizes "what should the pet do next" weighted randomness so
/// tuning behavior probabilities happens in one place.
/// </summary>
internal static class PetBehaviorPicker
{
    /// <summary>
    /// Picks a next state from the ground-based behaviors. Weighted so
    /// idling/walking are common, running is less common, and jumping is rare.
    /// </summary>
    public static IPetState PickNextGroundState(PetContext context)
    {
        double roll = context.Random.NextDouble();

        return roll switch
        {
            < 0.35 => new WalkingState(),
            < 0.55 => new RunningState(),
            < 0.85 => new IdleState(),
            < 0.93 => new SittingState(),
            < 0.97 => new JumpingState(),
            _ => new SleepingState()
        };
    }
}
