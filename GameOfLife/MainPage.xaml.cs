namespace GameOfLife;

public partial class MainPage : ContentPage
{

	public MainPage()
	{
		InitializeComponent();

		var drawable = new GraphicsDrawable();
		graphics.Drawable = drawable;
		graphics.StartInteraction += drawable.OnInteract;
	}

	
}

public class GraphicsDrawable : IDrawable
{
	private List<PointF> points = new List<PointF>();

	public void OnInteract(object sender, TouchEventArgs e)
	{
		points.Add(e.Touches.First());
		(sender as GraphicsView).Invalidate();
	}
    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
		canvas.FillColor = Colors.Blue;
        foreach (var p in points)
		{
			canvas.FillCircle(p, 4f);
		}
    }
}

