using System.Windows.Controls;
using PocketPal.Animation;
using PocketPal.Models;

using WpfImage = System.Windows.Controls.Image;

namespace PocketPal.Rendering;

public sealed class PetRenderer
{
    private readonly WpfImage _surface;
    private readonly Canvas _canvas;
    private readonly double _scale;

    public PetRenderer(WpfImage surface, Canvas canvas, double scale = 1.0)
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
