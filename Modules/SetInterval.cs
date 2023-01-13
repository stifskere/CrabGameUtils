using DateTimeOffset = System.DateTimeOffset;

namespace CrabGameUtils.Modules;

public delegate void IntervalFunc();
public class SetInterval
{
    private readonly IntervalFunc _func;
    private readonly System.TimeSpan _interval;
    private ulong _targetTime;
    public SetInterval(IntervalFunc func, System.TimeSpan interval)
    {
        _func = func;
        _interval = interval;
        _targetTime = (ulong)DateTimeOffset.Now.ToUnixTimeSeconds() + (ulong)interval.Milliseconds;
        RunFunc1();
    }

    public SetInterval(IntervalFunc func, int ms) : this(func, System.TimeSpan.FromMilliseconds(ms)) {}

    private void RunFunc1()
    {
        if ((ulong)DateTimeOffset.Now.ToUnixTimeMilliseconds() > _targetTime)
        {
            _func.Invoke();
            _targetTime += (ulong)_interval.Milliseconds;
        }
        RunFunc2();
    }

    private void RunFunc2() => RunFunc1();
}