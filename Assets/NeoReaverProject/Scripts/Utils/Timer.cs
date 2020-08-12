using System;
using Packages.EZRollback.Runtime.Scripts;
using UnityEngine;

public class Timer {
    float _maxTime = 0.0f;
    RollbackElement<float> _currentTime;

    // Return true the first frame that should be executed
    public bool ShouldExecute() {
        return _currentTime.value >= _maxTime;
    }

    public Timer(float maxTime) {
        _maxTime = maxTime;
        _currentTime = new RollbackElement<float>();
    }
    
    public void Simulate() {
        _currentTime.value += Time.fixedDeltaTime;
    }
    
    public void Reset() {
        _currentTime.value = 0.0f;
    }

    public float GetRemainingTime() {
        return _maxTime - _currentTime.value;
    }
    
    public void SetValueFromFrameNumber(int frameNumber) {
        _currentTime.SetValueFromFrameNumber(frameNumber);
    }
    
    public void DeleteFrames(int numFramesToDelete, RollbackManager.DeleteFrameMode deleteMode) { 
        _currentTime.DeleteFrames(numFramesToDelete, deleteMode);
    }

    public void SaveFrame() {
        _currentTime.SaveFrame();
    }
}
