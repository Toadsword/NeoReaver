using System.Collections;
using System.Collections.Generic;
using Packages.EZRollback.Runtime.Scripts;
using UnityEngine;

public class SpeedmoveRollback : RollbackBehaviour {
    public Vector3 direction;

    public override void Simulate() {
        transform.position = transform.position + (direction * Time.fixedDeltaTime);
    }

    public override void SetValueFromFrameNumber(int frameNumber) { }

    public override void DeleteFrames(int numFramesToDelete, RollbackManager.DeleteFrameMode deleteMode) { }

    public override void SaveFrame() { }
}
