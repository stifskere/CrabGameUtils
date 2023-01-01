using IDisposable = System.IDisposable;

namespace CrabGameUtils.Modules;

public delegate void RunThread();

public class MethodRunDelay : IDisposable
{
    private System.DateTime? _time;
    private RunThread? _method;
    
    public MethodRunDelay(System.TimeSpan time, RunThread method)
    {
        _time = System.DateTime.Now + time;
        _method = method;
    }

    public void WaitAndRun()
    {
        if (_time == null || !(System.DateTime.Now >= _time)) return;
        _method.Invoke();
        Dispose();
    }

    public void Dispose()
    {
        _time = null;
        _method = null;   
    }
}