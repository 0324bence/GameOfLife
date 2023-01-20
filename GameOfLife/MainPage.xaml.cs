using System.Diagnostics;

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
        //view.MoveHoverInteraction += drawable.OnClick;
	}
}

public class Drawable : IDrawable
{
	public static readonly int s_gridSpacing = 10;
	public int CanvasSizePX = 0;

	public void OnClick(object sender, TouchEventArgs e)
	{
		//points.Add(e.Touches.First());
		var view = (sender as GraphicsView);
		view.Invalidate();
        Debug.WriteLine($"{e.Touches.First().X}, {e.Touches.First().Y}");
	}
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
        canvas.FillColor = Colors.Tomato;
        canvas.FillRectangle(s_gridSpacing, s_gridSpacing, s_gridSpacing, s_gridSpacing);

        canvas.StrokeColor = Colors.Black;

        for (int i = 0; i <= CanvasSizePX; i += s_gridSpacing)
        {
            canvas.DrawLine(i, 0, i, CanvasSizePX);
        }

        for (int i = 0; i <= CanvasSizePX; i += s_gridSpacing)
        {
            canvas.DrawLine(0, i, CanvasSizePX, i);
        }
    }
}

