using System;
using Packages.EZRollback.Runtime.Scripts;
using UnityEngine;

public class Timer
{
    RollbackElement<float> _currentTime;
    float _maxTime;
    
    public int Interval { get; set; }

    public bool ShouldExecute()
    {
        return _currentTime.value >= _maxTime;
    }

    public Timer(float maxTime) {
        _maxTime = maxTime;
        _currentTime = new RollbackElement<float>();
        _currentTime.value = 0.0f;
    }
    
    public void AddTime(float timeToAdd) {
        _currentTime.value += timeToAdd;
    }
    
    public void Reset() {
        _currentTime.value = 0.0f;
    }
    
    public void SetValueFromFrameNumber(int frameNumber) {
        _currentTime.SetValueFromFrameNumber(frameNumber);
    }
    
    public void DeleteFrames(int numFramesToDelete, bool firstFrames) { 
        _currentTime.DeleteFrames(numFramesToDelete, firstFrames);
    }

    public void SaveFrame() {
        _currentTime.SaveFrame();
    }
}
