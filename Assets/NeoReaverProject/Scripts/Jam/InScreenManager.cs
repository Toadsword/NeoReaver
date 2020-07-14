using System;
using System.Collections;
using System.Collections.Generic;
using Packages.EZRollback.Runtime.Scripts;
using UnityEngine;

public class InScreenManager : IRollbackBehaviour {

    public static InScreenManager _instance;

    void Awake() {
        if (_instance == null) {
            _instance = this;
        } else {
            Destroy(this);
        }
    }

    List<Transform> _transformsToTrack = new List<Transform>();

    [SerializeField] Transform _bottomLeftPosition;
    [SerializeField] Transform _topRightPosition;
    
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        _transformsToTrack = new List<Transform>();
    }

    public void RegisterObject(GameObject obj) {
        _transformsToTrack.Add(obj.transform);
    }

    public override void Simulate() {
        foreach (Transform transform in _transformsToTrack) {
            Vector3 newPos = transform.position;
            if (newPos.x < _bottomLeftPosition.position.x || newPos.x > _topRightPosition.position.x) {
                newPos.x = -newPos.x;
            }
            if (newPos.y < _bottomLeftPosition.position.y || newPos.y > _topRightPosition.position.y) {
                newPos.y = -newPos.y;
            }
        }
    }

    public override void SetValueFromFrameNumber(int frameNumber) { }

    public override void DeleteFrames(int numFramesToDelete, bool firstFrames) { }

    public override void SaveFrame() { }
}
