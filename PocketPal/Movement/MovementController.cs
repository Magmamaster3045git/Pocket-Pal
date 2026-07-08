using PocketPal.Models;

namespace PocketPal.Movement;

public sealed class MovementController
{
    public Vector2D Position { get; set; }

    public Vector2D Velocity { get; set; }

    public Direction Direction { get; set; } = Direction.Right;

    public double AreaWidth { get; set; }

    public double GroundY { get; set; }

    public double SpriteWidth { get; set; } = 64;


    // Click-to-move target
    public double? TargetX { get; private set; }

    public bool HasTarget => TargetX.HasValue;


    public bool IsGrounded => Position.Y >= GroundY;


    public event Action? HitLeftEdge;
    public event Action? HitRightEdge;


    public void SetTarget(double x)
    {
        TargetX = Math.Clamp(
            x,
            0,
            AreaWidth - SpriteWidth
        );


        Direction = TargetX.Value >= Position.X
            ? Direction.Right
            : Direction.Left;
    }


    public void ClearTarget()
    {
        TargetX = null;
    }


    public void MoveTowardsTarget(
        double speedPixelsPerSecond,
        double deltaSeconds)
    {
        if (!TargetX.HasValue)
            return;


        double distance = TargetX.Value - Position.X;


        // Reached destination
        if (Math.Abs(distance) < 2)
        {
            Position = new Vector2D(
                TargetX.Value,
                GroundY
            );

            TargetX = null;
            return;
        }


        double direction = Math.Sign(distance);


        Position = new Vector2D(
            Position.X + direction * speedPixelsPerSecond * deltaSeconds,
            GroundY
        );


        Direction = direction > 0
            ? Direction.Right
            : Direction.Left;


        ClampToBounds();
    }


    public void MoveHorizontal(
        double speedPixelsPerSecond,
        double deltaSeconds)
    {
        double signed =
            Direction == Direction.Right
            ? speedPixelsPerSecond
            : -speedPixelsPerSecond;


        var pos = Position;

        pos.X += signed * deltaSeconds;

        Position = pos;

        ClampToBoundsAndFlip();
    }


    private void ClampToBounds()
    {
        var pos = Position;


        if (pos.X < 0)
            pos.X = 0;


        if (pos.X + SpriteWidth > AreaWidth)
            pos.X = AreaWidth - SpriteWidth;


        Position = pos;
    }


    private void ClampToBoundsAndFlip()
    {
        var pos = Position;


        if (pos.X <= 0)
        {
            pos.X = 0;
            Direction = Direction.Right;
            HitLeftEdge?.Invoke();
        }
        else if (pos.X + SpriteWidth >= AreaWidth)
        {
            pos.X = AreaWidth - SpriteWidth;
            Direction = Direction.Left;
            HitRightEdge?.Invoke();
        }


        Position = pos;
    }
}
