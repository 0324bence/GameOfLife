using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using System.Diagnostics;
using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace GameOfLife;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();

        var drawable = new Drawable();
        drawable.CanvasSizePX = (int)view.WidthRequest;
        view.Drawable = drawable;
		view.StartInteraction += drawable.OnClick;
        view.MoveHoverInteraction += drawable.OnHoverMove;
        view.EndHoverInteraction += drawable.OnHoverEnd;
    }
}

struct Pos
{
    public int Col;
    public int Row;

    public Pos(int col, int row)
    {
        Col = col;
        Row = row;
    }
}

public class Drawable : IDrawable
{
    public static readonly int GRID_SIZE = 50;

    private int _canvasSizePx = 0;
    public int _gridSpacing = 0;
    public int CanvasSizePX
    {
        get => _canvasSizePx;
        set {
            _canvasSizePx = value;
            _gridSpacing = _canvasSizePx / GRID_SIZE;
        }
    }

    private Pos _previewPos = new Pos(-1, -1);

	public void OnClick(object sender, TouchEventArgs e)
    {
        Debug.WriteLine($"Click: {e.Touches.First().X}, {e.Touches.First().Y}");

        var view = (sender as GraphicsView);
        view.Invalidate();
    }

    public void OnHoverMove(object sender, TouchEventArgs args)
    {
        Debug.WriteLine($"Hover: {args.Touches.First().X}, {args.Touches.First().Y}");

        _previewPos.Col = (int)((args.Touches.First().X) / _gridSpacing);
        _previewPos.Row = (int)((args.Touches.First().Y) / _gridSpacing);

        var view = (sender as GraphicsView);
        view.Invalidate();
    }
    public void OnHoverEnd(object sender, EventArgs args)
    {
        Debug.WriteLine($"Hover end");

        _previewPos = new Pos(-1, -1);

        var view = (sender as GraphicsView);
        view.Invalidate();
    }

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.StrokeSize = 1;
        canvas.StrokeColor = Colors.Black;

        for (int i = 0; i <= CanvasSizePX; i += _gridSpacing)
        {
            canvas.DrawLine(i, 0, i, CanvasSizePX);
        }

        for (int i = 0; i <= CanvasSizePX; i += _gridSpacing)
        {
            canvas.DrawLine(0, i, CanvasSizePX, i);
        }

        canvas.StrokeSize = 3;
        canvas.StrokeColor = Colors.Salmon;
        canvas.DrawRectangle(_previewPos.Col * _gridSpacing, _previewPos.Row * _gridSpacing, _gridSpacing, _gridSpacing);
    }
}

