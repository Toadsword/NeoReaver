using System;
using System.Collections;
using System.Collections.Generic;
using Packages.EZRollback.Runtime.Scripts;
using UnityEngine;

public class SelfDestruct : IRollbackBehaviour {
    [SerializeField] int _timeBeforeSelfDestruct = 10;

    Timer _timer;
    
    new void Start() {
        base.Start();
        
        _timer = new Timer(_timeBeforeSelfDestruct);
        SetGameObjectActive();
    }

    public void SetGameObjectActive() {
        gameObject.SetActive(true);

        _timer.Reset();
    }
    
    void Update() {
        UpdateTimer(Time.deltaTime);
    }

    public override void Simulate() {
        UpdateTimer(Time.fixedDeltaTime);
    }

    private void UpdateTimer(float deltaTime) {
        _timer.AddTime(deltaTime);
        if (_timer.ShouldExecute()) {
            gameObject.SetActive(false);
        }
    }
    
    public override void SetValueFromFrameNumber(int frameNumber) {
        _timer.SetValueFromFrameNumber(frameNumber);
    }

    public override void DeleteFrames(int numFramesToDelete, bool firstFrames) {
        _timer.DeleteFrames(numFramesToDelete, firstFrames);
    }

    public override void SaveFrame() {
        _timer.SaveFrame();
    }
}
