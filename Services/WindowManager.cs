using System.Linq;
using Dalamud.Interface.Windowing;
using LeveHelper.Extensions;

namespace LeveHelper.Services;

public class WindowManager : WindowSystem, IDisposable
{
    public WindowManager(string? imNamespace = null) : base(imNamespace)
    {
    }

    public T? GetWindow<T>() where T : Window
    {
        return Windows.OfType<T>().FirstOrDefault();
    }

    public void OpenWindow<T>() where T : Window, new()
    {
        if (!Windows.FindFirst(w => w.GetType() == typeof(T), out var window))
        {
            AddWindow(window = new T());
        }

        window.IsOpen = true;
    }

    public void CloseWindow<T>() where T : Window
    {
        if (Windows.FindFirst(w => w.GetType() == typeof(T), out var window))
        {
            (window as IDisposable)?.Dispose();
            RemoveWindow(window);
        }
    }

    public void ToggleWindow<T>() where T : Window, new()
    {
        if (GetWindow<T>() is { } window)
        {
            CloseWindow<T>();
        }
        else
        {
            OpenWindow<T>();
        }
    }

    public void Dispose()
    {
        foreach (var window in Windows)
        {
            (window as IDisposable)?.Dispose();
        }

        RemoveAllWindows();
    }
}
