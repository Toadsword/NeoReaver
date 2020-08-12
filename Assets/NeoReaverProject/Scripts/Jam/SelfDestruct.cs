using System;
using System.Collections;
using System.Collections.Generic;
using Packages.EZRollback.Runtime.Scripts;
using UnityEngine;

public class SelfDestruct : RollbackBehaviour {
    [SerializeField] float _timeBeforeSelfDestruct = 1.5f;

    RollbackTimer _rollbackTimer;

    new void Awake() {
        base.Awake();
        
        _rollbackTimer = new RollbackTimer(_timeBeforeSelfDestruct);
        SetGameObjectActive();
    }

    public void SetGameObjectActive() {
        gameObject.SetActive(true);
        _rollbackTimer.Reset();
    }
    
    public override void Simulate() {
        _rollbackTimer.Simulate();
        if (_rollbackTimer.ShouldExecute()) {
            gameObject.SetActive(false);
        }
    }
    
    public override void SetValueFromFrameNumber(int frameNumber) {
        _rollbackTimer.SetValueFromFrameNumber(frameNumber);
    }

    public override void DeleteFrames(int numFramesToDelete, RollbackManager.DeleteFrameMode deleteMode) {
        _rollbackTimer.DeleteFrames(numFramesToDelete, deleteMode);
    }

    public override void SaveFrame() {
        _rollbackTimer.SaveFrame();
    }
}
