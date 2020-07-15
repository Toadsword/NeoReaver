using System;
using Packages.EZRollback.Runtime.Scripts;
using UnityEngine;

struct TimerInfos {
    public float currentTime;
    public bool enabled;

    public void Reset() {
        currentTime = 0.0f;
        enabled = true;
    }
}

public class Timer {
    float _maxTime = 0.0f;
    RollbackElement<TimerInfos> _timerInfos;

    // Return true the first frame that should be executed
    public bool ShouldExecute() {
        bool result = false;
        
        if(_timerInfos.value.enabled) 
            result = _timerInfos.value.currentTime >= _maxTime;
        
        if (result) 
            _timerInfos.value.enabled = false;
        
        return result;
    }

    public Timer(float maxTime) {
        _maxTime = maxTime;
        _timerInfos = new RollbackElement<TimerInfos>();
    }
    
    public void Simulate() {
        _timerInfos.value.currentTime += Time.fixedDeltaTime;
    }
    
    public void Reset() {
        _timerInfos.value.Reset();
    }
    
    public void SetValueFromFrameNumber(int frameNumber) {
        _timerInfos.SetValueFromFrameNumber(frameNumber);
    }
    
    public void DeleteFrames(int numFramesToDelete, bool firstFrames) { 
        _timerInfos.DeleteFrames(numFramesToDelete, firstFrames);
    }

    public void SaveFrame() {
        _timerInfos.SaveFrame();
    }
}
