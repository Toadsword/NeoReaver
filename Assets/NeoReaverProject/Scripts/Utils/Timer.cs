using UnityEngine;

public class Timer {
    
    private float _lastExecutionTime = Time.time;
    private bool _shouldExecute;

    public float Interval { get; set; }

    public bool IsEnabled { get; set; }

    public bool ShouldExecute 
    {
        get { return (this.IsEnabled && (this._shouldExecute || (Time.time - this._lastExecutionTime > this.Interval))); } 
        set { this._shouldExecute = value; } 
    }

    public Timer(float interval)
    {
        this.IsEnabled = true;
        this.Interval = interval;
    }
    
    public void Reset()
    {
        this._shouldExecute = false;
        this._lastExecutionTime = Time.time;
    }

    public float GetRemainingTime() {
        return Interval - (Time.time - this._lastExecutionTime);
    }
}

