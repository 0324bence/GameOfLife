using Microsoft.Maui;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;

namespace GameOfLife;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

        var mainpage = new MainPage();

        Microsoft.Maui.Handlers.WindowHandler.Mapper.AppendToMapping(nameof(IWindow), (handler, view) =>
        {
            int WINDOW_WIDTH = 1120;
            int WINDOW_HEIGHT = 950;
            var mauiWindow = handler.VirtualView;
            var nativeWindow = handler.PlatformView;
            nativeWindow.Activate();
            IntPtr windowHandle = WinRT.Interop.WindowNative.GetWindowHandle(nativeWindow);
            WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowHandle);
            AppWindow appWindow = Microsoft.UI.Windowing.AppWindow.GetFromWindowId(windowId);
            int height = (int)(DeviceDisplay.Current.MainDisplayInfo.Height);
            int width = (int)DeviceDisplay.Current.MainDisplayInfo.Width;
            var rect = new RectInt32((width / 2) - WINDOW_WIDTH / 2, (height / 2) - WINDOW_HEIGHT / 2, WINDOW_WIDTH, WINDOW_HEIGHT);
            //appWindow.Resize(new SizeInt32(920, 950));
            appWindow.MoveAndResize(rect);
            var presenter = appWindow.Presenter as OverlappedPresenter;
            presenter.IsResizable = false;
        });

        MainPage = mainpage;

    }
}
