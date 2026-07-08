using PocketPal.Animation;
using PocketPal.Assets;
using PocketPal.Models;
using PocketPal.Movement;
using PocketPal.Rendering;
using PocketPal.StateMachine;
using PocketPal.StateMachine.States;

namespace PocketPal.Core;

public sealed class PetEngine
{
    public MovementController Movement { get; }
    public AnimationLibrary Animations { get; }
    public AnimationPlayer Player { get; }
    public PetStateMachine States { get; }

    private readonly PetRenderer _renderer;


    public PetEngine(
        MovementController movement,
        AnimationLibrary animations,
        PetRenderer renderer,
        int framesPerSecond,
        Random random,
        bool staticMode = false)
    {
        Movement = movement;
        Animations = animations;
        _renderer = renderer;

        Player = new AnimationPlayer(framesPerSecond);


        var context = new PetContext(
            movement,
            animations,
            random)
        {
            StaticMode = staticMode
        };


        States = new PetStateMachine(
            context,
            new IdleState());
    }


    public void Update(double deltaSeconds)
    {
        // If a target exists, ensure the pet is running.
        // RunningState owns the actual movement.
        if (Movement.HasTarget &&
            States.CurrentType != PetStateType.Running)
        {
            States.ForceTransition(
                new RunningState());
        }


        // State machine controls:
        // - walking
        // - running
        // - sitting
        // - sleeping
        // - static mode behaviour
        States.Update(deltaSeconds);


        AnimationClip clip = Animations.Resolve(
            States.CurrentType,
            Movement.Direction);


        Player.Play(clip);
        Player.Update(deltaSeconds);


        _renderer.DrawFrame(Player);
        _renderer.PositionSprite(Movement.Position);
    }
}
