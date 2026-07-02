using System.Windows.Controls;
using PocketPal.Animation;
using PocketPal.Models;

namespace PocketPal.Rendering;

/// <summary>
/// Draws whatever AnimationPlayer/MovementController currently say to draw.
/// Deliberately has zero decision-making logic - it is a dumb "paint what
/// I'm told" layer so rendering stays swappable (e.g. later replaced by a
/// Win2D/SkiaSharp surface) without touching game logic.
/// </summary>
public sealed class PetRenderer
{
    private readonly Image _surface;
    private readonly Canvas _canvas;
    private readonly double _scale;

    public PetRenderer(Image surface, Canvas canvas, double scale = 1.0)
    {
        _surface = surface;
        _canvas = canvas;
        _scale = scale <= 0 ? 1.0 : scale;
    }

    public void DrawFrame(AnimationPlayer player)
    {
        if (player.CurrentFrame is null) return;

        _surface.Source = player.CurrentFrame;
        _surface.Width = player.CurrentClip!.FrameWidth * _scale;
        _surface.Height = player.CurrentClip.FrameHeight * _scale;
    }

    public void PositionSprite(Vector2D position)
    {
        Canvas.SetLeft(_surface, position.X);
        Canvas.SetTop(_surface, position.Y);
    }
}
