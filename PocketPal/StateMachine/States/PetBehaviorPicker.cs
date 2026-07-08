namespace PocketPal.StateMachine.States;

/// <summary>
/// Centralizes "what should the pet do next" weighted randomness so
/// tuning behavior probabilities happens in one place.
/// </summary>
internal static class PetBehaviorPicker
{
    /// <summary>
    /// Picks the next state from normal ground behaviors.
    /// </summary>
    public static IPetState PickNextGroundState(PetContext context)
    {
        // Static Mode:
        // The pet does not wander.
        // It only rests by sitting or sleeping.
        if (context.StaticMode)
        {
            return context.Random.Next(2) switch
            {
                0 => new SittingState(),
                _ => new SleepingState()
            };
        }


        // Normal AI behaviour.
        // Weighted:
        // walking/running are common,
        // sitting/sleeping are less common.
        double roll = context.Random.NextDouble();

        return roll switch
        {
            < 0.35 => new WalkingState(),
            < 0.55 => new RunningState(),
            < 0.85 => new IdleState(),
            < 0.93 => new SittingState(),
            _ => new SleepingState()
        };
    }
}
