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

struct Cell
{
    public bool IsAlive;

    public static implicit operator bool(in Cell c)
    {
        return c.IsAlive;
    }

    public void Toggle() => IsAlive = !IsAlive;
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

    private Cell[][] _cells = new Cell[GRID_SIZE][];

    public Drawable()
    {
        for (int i = 0; i < GRID_SIZE; ++i)
        {
            _cells[i] = new Cell[GRID_SIZE];
        }
    }

	public void OnClick(object sender, TouchEventArgs e)
    {
        int x = (int)e.Touches.First().X;
        int y = (int)e.Touches.First().Y;
        Debug.WriteLine($"Click: {x}, {y}");
        _cells[x / _gridSpacing][y / _gridSpacing].Toggle();

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
        canvas.FillColor = Colors.Red;
        for (int y = 0; y < GRID_SIZE; ++y)
        {
            for (int x = 0; x < GRID_SIZE; ++x)
            {
                if (_cells[x][y])
                {
                    canvas.FillRectangle(x * _gridSpacing, y * _gridSpacing, _gridSpacing, _gridSpacing);
                }
            }
        }

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

