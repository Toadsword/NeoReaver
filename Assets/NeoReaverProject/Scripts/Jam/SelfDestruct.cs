using System;
using System.Collections;
using System.Collections.Generic;
using Packages.EZRollback.Runtime.Scripts;
using UnityEngine;

public class SelfDestruct : RollbackBehaviour {
    [SerializeField] float _timeBeforeSelfDestruct = 1.5f;

    Timer _timer;

    new void Awake() {
        base.Awake();
        
        _timer = new Timer(_timeBeforeSelfDestruct);
        SetGameObjectActive();
    }

    public void SetGameObjectActive() {
        gameObject.SetActive(true);
        _timer.Reset();
    }
    
    public override void Simulate() {
        _timer.Simulate();
        if (_timer.ShouldExecute()) {
            gameObject.SetActive(false);
        }
    }

    private void UpdateTimer(float deltaTime) {
    }
    
    public override void SetValueFromFrameNumber(int frameNumber) {
        _timer.SetValueFromFrameNumber(frameNumber);
    }

    public override void DeleteFrames(int numFramesToDelete, RollbackManager.DeleteFrameMode deleteMode) {
        _timer.DeleteFrames(numFramesToDelete, deleteMode);
    }

    public override void SaveFrame() {
        _timer.SaveFrame();
    }
}
