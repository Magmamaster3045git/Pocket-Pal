using PocketPal.Models;

namespace PocketPal.Movement;

/// <summary>
/// Owns the pet's physical position/velocity and screen-bound clamping.
/// Deliberately knows nothing about states or animations - it only moves
/// a point around, so it stays simple to unit test and reuse.
/// </summary>
public sealed class MovementController
{
    /// <summary>Position of the pet's bottom-left corner, in device-independent pixels, relative to the current monitor's work area.</summary>
    public Vector2D Position { get; set; }

    public Vector2D Velocity { get; set; }

    public Direction Direction { get; set; } = Direction.Right;

    // NEW: Target position for click-to-move
    public double? TargetX { get; set; }

    public bool HasTarget => TargetX.HasValue;

    /// <summary>Width of the bounding area the pet walks within (typically the monitor's work-area width).</summary>
    public double AreaWidth { get; set; }

    /// <summary>Y coordinate of the ground line (bottom of the sprite when standing).</summary>
    public double GroundY { get; set; }

    /// <summary>Sprite width, used for edge-of-screen collision.</summary>
    public double SpriteWidth { get; set; } = 64;

    public bool IsGrounded => Position.Y >= GroundY;

    public event Action? HitLeftEdge;
    public event Action? HitRightEdge;

    /// <summary>Moves horizontally at the given signed speed (pixels/sec, sign ignored - direction controls sign).</summary>
    public void MoveHorizontal(double speedPixelsPerSecond, double deltaSeconds)
    {
        double signed = Direction == Direction.Right ? speedPixelsPerSecond : -speedPixelsPerSecond;
        var pos = Position;
        pos.X += signed * deltaSeconds;
        Position = pos;
        ClampToBoundsAndFlip();
    }

    // NEW: Smooth movement toward a clicked target
    public void MoveTowardsTarget(double speedPixelsPerSecond, double deltaSeconds)
    {
        if (!TargetX.HasValue)
            return;

        var pos = Position;
        double target = TargetX.Value;

        // Close enough -> stop
        if (Math.Abs(pos.X - target) < 2)
        {
            pos.X = target;
            Position = pos;
            TargetX = null;
            return;
        }

        Direction = pos.X < target
            ? Direction.Right
            : Direction.Left;

        double move = speedPixelsPerSecond * deltaSeconds;

        if (Direction == Direction.Right)
            pos.X = Math.Min(pos.X + move, target);
        else
            pos.X = Math.Max(pos.X - move, target);

        Position = pos;

        ClampToBoundsAndFlip();
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
