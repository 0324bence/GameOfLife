﻿using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Dynamic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace GameOfLife;

public partial class MainPage : ContentPage
{

    Drawable drawable;
    bool isTimerStarted = false;
    int intervalCounter = 0;
    public MainPage()
	{
		InitializeComponent();

        drawable = new(this);

        drawable.CanvasSizePX = (int)view.WidthRequest;
        view.Drawable = drawable;
        drawable.localView = view;
		view.StartInteraction += drawable.OnClick;
        view.EndInteraction += drawable.OnClickEnd;
        view.MoveHoverInteraction += drawable.OnHoverMove;
        view.EndHoverInteraction += drawable.OnHoverEnd;
        CountLabel.Text = $"Iteration: {intervalCounter}";
    }

    public void ToggleTimer()
    {
#pragma warning disable CS4014
        if (!isTimerStarted) {
            StartButton.Text = "Leállítás";
            isTimerStarted = true;
            IntervalMethod();
        } else {
            StartButton.Text = "Indítás";
            isTimerStarted = false;
        }
#pragma warning restore CS4014
    }

    public async Task IntervalMethod()
    {
#pragma warning disable CS4014
        drawable.StepSimulation();
        view.Invalidate();
        intervalCounter++;
        CountLabel.Text = $"Iteration: {intervalCounter}";
        await Task.Delay(50);
        if (isTimerStarted) IntervalMethod();
#pragma warning restore CS4014
    }

    public void StartButtonClick(object sender, EventArgs e) {
        ToggleTimer();
    }

    public bool ShowGrid = true;

    private void ShowGridCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        ShowGrid = e.Value;
        view.Invalidate();
    }

    public partial class Drawable : IDrawable { }
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
    public uint Age;

    public static implicit operator bool(in Cell c) => c.IsAlive;
    public static implicit operator int(in Cell c) => (c.IsAlive ? 1 : 0);

    public void Toggle() => IsAlive = !IsAlive;

    public void StepAge()
    {
        Age += 5;
    }

    public Color GetColor()
    {
        return Color.FromHsla(Age%360/360.0f, 1, 0.5, 1);
    }
}

public partial class MainPage
{
    public partial class Drawable : IDrawable
    {
        public static readonly int GRID_SIZE = 50;
        public GraphicsView localView;

        private int _canvasSizePx = 0;
        public int _gridSpacing = 0;
        public int CanvasSizePX
        {
            get => _canvasSizePx;
            set
            {
                _canvasSizePx = value;
                _gridSpacing = _canvasSizePx / GRID_SIZE;
            }
        }

        private Pos _previewPos = new Pos(-1, -1);
        private bool _isMouseDown = false;

        private Cell[][] _cells = new Cell[GRID_SIZE][];

        private MainPage _mainPage;

        public Drawable(MainPage mainPage)
        {
            for (int i = 0; i < GRID_SIZE; ++i)
            {
                _cells[i] = new Cell[GRID_SIZE];
            }
            _mainPage = mainPage;
        }

        public void OnClick(object sender, TouchEventArgs e)
        {
            int x = (int)e.Touches.First().X;
            int y = (int)e.Touches.First().Y;
            Debug.WriteLine($"Click: {x}, {y}");
            _cells[x / _gridSpacing][y / _gridSpacing].Toggle();
            _cells[x / _gridSpacing][y / _gridSpacing].Age = 0;
            _isMouseDown = true;

            var view = (sender as GraphicsView);
            view.Invalidate();
        }

        public void OnClickEnd(object sender, TouchEventArgs e)
        {
            int x = (int)e.Touches.First().X;
            int y = (int)e.Touches.First().Y;
            Debug.WriteLine($"Click end: {x}, {y}");

            _isMouseDown = false;
        }

        public void OnHoverMove(object sender, TouchEventArgs args)
        {
            Debug.WriteLine($"Hover: {args.Touches.First().X}, {args.Touches.First().Y}");

            _previewPos.Col = (int)((args.Touches.First().X) / _gridSpacing);
            _previewPos.Row = (int)((args.Touches.First().Y) / _gridSpacing);

            if (_isMouseDown)
            {
                int x = (int)args.Touches.First().X;
                int y = (int)args.Touches.First().Y;
                int col = x / _gridSpacing;
                int row = y / _gridSpacing;
                if (col >= 0 && col < GRID_SIZE && row >= 0 && row < GRID_SIZE)
                {
                    _cells[col][row].IsAlive = true;
                    _cells[col][row].Age = 0;
                }
            }

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
            canvas.FillColor = Colors.White;
            canvas.FillRectangle(0, 0, CanvasSizePX, CanvasSizePX);

            for (int y = 0; y < GRID_SIZE; ++y)
            {
                for (int x = 0; x < GRID_SIZE; ++x)
                {
                    if (_cells[x][y])
                    {
                        canvas.FillColor = _cells[x][y].GetColor();
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
            //Debug.WriteLine("refresh");
        }

        public int CountNeighbours(int x, int y)
        {
            int count = 0;
            if (y - 1 >= 0)
            {
                if (x - 1 >= 0)
                    count += _cells[x - 1][y - 1];
                count += _cells[x + 0][y - 1];
                if (x + 1 < GRID_SIZE)
                    count += _cells[x + 1][y - 1];
            }

            if (x - 1 >= 0)
                count += _cells[x - 1][y + 0];
            if (x + 1 < GRID_SIZE)
                count += _cells[x + 1][y + 0];

            if (y + 1 < GRID_SIZE)
            {
                if (x - 1 >= 0)
                    count += _cells[x - 1][y + 1];
                count += _cells[x + 0][y + 1];
                if (x + 1 < GRID_SIZE)
                    count += _cells[x + 1][y + 1];
            }

            return count;
        }

        public void StepSimulation()
        {
            Debug.WriteLine("Step");

            var newCells = _cells.Select(x => x.ToArray()).ToArray();
            for (int i = 0; i < GRID_SIZE; i++)
            {
                for (int j = 0; j < GRID_SIZE; j++)
                {
                    var count = CountNeighbours(i, j);

                    if (count < 2)
                    {
                        newCells[i][j].IsAlive = false;
                    }
                    else if (count == 2)
                    {
                        // NOP
                    }
                    else if (count == 3)
                    {
                        newCells[i][j].IsAlive = true;
                        newCells[i][j].Age = 0;
                    }
                    else
                    {
                        newCells[i][j].IsAlive = false;
                    }

                    newCells[i][j].StepAge();
                }
            }

            _cells = newCells;
        }
    }
}

