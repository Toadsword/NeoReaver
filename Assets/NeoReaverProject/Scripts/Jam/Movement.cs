using System;
using System.Collections;
using System.Collections.Generic;
using Packages.EZRollback.Runtime.Scripts;
using UnityEngine;

public class Movement : RollbackBehaviour {

    public Vector2 speed;
    void Update() {
        Move(Time.deltaTime);
    }

    public override void Simulate() {
        Move(Time.fixedDeltaTime);
    }

    public override void SetValueFromFrameNumber(int frameNumber) { }

    public override void DeleteFrames(int numFramesToDelete, RollbackManager.DeleteFrameMode deleteMode) { }

    public override void SaveFrame() { }

    void Move(float deltaTime) {
        transform.position = transform.position + ((Vector3)speed * deltaTime);
    }
}
