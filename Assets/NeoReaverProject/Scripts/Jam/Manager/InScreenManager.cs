﻿using System;
using System.Collections;
using System.Collections.Generic;
using Packages.EZRollback.Runtime.Scripts;
using Packages.EZRollback.Runtime.Scripts.Utils;
using UnityEngine;

public class InScreenManager : RollbackBehaviour {

    public static InScreenManager _instance;

    new void Awake() {
        base.Awake();
        
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
        }
    }

    readonly List<Transform> _transformsToTrack = new List<Transform>();

    [SerializeField] Transform _bottomLeftPosition;
    [SerializeField] Transform _topRightPosition;
    
    public void RegisterObject(Transform transformObj) {
        _transformsToTrack.Add(transformObj);
    }

    public void UnregisterObject(Transform transformObj) {
        if (_transformsToTrack.Contains(transformObj)) {
            _transformsToTrack.Remove(transformObj);
        }
    }

    public override void Simulate() {
        foreach (Transform trackingTransform in _transformsToTrack) {
            if (!trackingTransform.gameObject.activeSelf)
                continue;
            
            Vector3 newPos = trackingTransform.position;
            if (newPos.x < _bottomLeftPosition.position.x || newPos.x > _topRightPosition.position.x) {
                newPos.x = -newPos.x;
            }
            if (newPos.y < _bottomLeftPosition.position.y || newPos.y > _topRightPosition.position.y) {
                newPos.y = -newPos.y;
            }

            trackingTransform.position = newPos;
        }
    }

    public override void SetValueFromFrameNumber(int frameNumber) { }

    public override void DeleteFrames(int numFramesToDelete, RollbackManager.DeleteFrameMode deleteMode) { }

    public override void SaveFrame() { }
}