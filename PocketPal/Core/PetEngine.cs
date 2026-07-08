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


        // Start directly in resting mode if Static Mode is enabled.
        States = new PetStateMachine(
            context,
            staticMode
                ? new SittingState()
                : new IdleState());
    }


    public void Update(double deltaSeconds)
    {
        // Taskbar click movement.
        // This is the only thing allowed to move a static pet.
        if (Movement.HasTarget &&
            States.CurrentType != PetStateType.Running)
        {
            States.ForceTransition(
                new RunningState());
        }


        States.Update(deltaSeconds);


        // Safety lock:
        // Static Mode should NEVER end up in walking/running/idle
        // unless it is actively moving to a clicked destination.
        if (States.Context.StaticMode &&
            !Movement.HasTarget)
        {
            if (States.CurrentType == PetStateType.Idle ||
                States.CurrentType == PetStateType.Walking ||
                States.CurrentType == PetStateType.Running)
            {
                States.ForceTransition(
                    States.Context.Random.Next(2) == 0
                        ? new SittingState()
                        : new SleepingState());
            }
        }


        AnimationClip clip = Animations.Resolve(
            States.CurrentType,
            Movement.Direction);


        Player.Play(clip);

        Player.Update(deltaSeconds);


        _renderer.DrawFrame(Player);

        _renderer.PositionSprite(
            Movement.Position);
    }
}
