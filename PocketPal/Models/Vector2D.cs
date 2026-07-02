namespace PocketPal.Models;

/// <summary>
/// Minimal mutable 2D vector used for pet position and velocity.
/// Screen-space: X increases rightward, Y increases downward (WPF convention).
/// </summary>
public struct Vector2D
{
    public double X;
    public double Y;

    public Vector2D(double x, double y)
    {
        X = x;
        Y = y;
    }

    public static Vector2D Zero => new(0, 0);

    public static Vector2D operator +(Vector2D a, Vector2D b) => new(a.X + b.X, a.Y + b.Y);
    public static Vector2D operator -(Vector2D a, Vector2D b) => new(a.X - b.X, a.Y - b.Y);
    public static Vector2D operator *(Vector2D a, double scalar) => new(a.X * scalar, a.Y * scalar);

    public override string ToString() => $"({X:F1}, {Y:F1})";
}
