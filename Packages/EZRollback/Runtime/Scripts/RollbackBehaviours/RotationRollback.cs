using System;
using UnityEngine;

namespace Packages.EZRollback.Runtime.Scripts.RollbackBehaviours {


[Serializable]
public class RollbackElementQuaternion : RollbackElement<Quaternion> { }

public class RotationRollback : RollbackBehaviour {
    [SerializeField] RollbackElementQuaternion rotationRB = new RollbackElementQuaternion();
    
    public override void Simulate() {
    }

    public override void SetValueFromFrameNumber(int frameNumber) {
        rotationRB.SetValueFromFrameNumber(frameNumber);
        transform.rotation = rotationRB.value;
    }

    public override void DeleteFrames(int numFramesToDelete, RollbackManager.DeleteFrameMode deleteMode) {
        rotationRB.DeleteFrames(numFramesToDelete, deleteMode);
    }

    public override void SaveFrame() {
        rotationRB.SetAndSaveValue(transform.rotation);
    }
}
}