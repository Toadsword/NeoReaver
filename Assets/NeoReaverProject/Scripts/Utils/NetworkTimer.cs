using System;

public class NetworkTimer
{
    private int _lastExecutionTime = Environment.TickCount;
    private bool _shouldExecute;

    public int Interval { get; set; }

    public bool IsEnabled { get; set; }

    public bool ShouldExecute 
    {
        get { return (this.IsEnabled && (this._shouldExecute || (Environment.TickCount - this._lastExecutionTime > this.Interval))); } 
        set { this._shouldExecute = value; } 
    }

    public NetworkTimer(int interval)
    {
        this.IsEnabled = true;
        this.Interval = interval;
    }
    
    public void Reset()
    {
        this._shouldExecute = false;
        this._lastExecutionTime = Environment.TickCount;
    }
}