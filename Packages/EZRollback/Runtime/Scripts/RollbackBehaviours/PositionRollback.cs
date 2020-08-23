using System;
using Packages.EZRollback.Runtime.Scripts;
using UnityEngine;

namespace Packages.EZRollback.Runtime.Scripts.RollbackBehaviours {

[Serializable]
public class RollbackElementVector3 : RollbackElement<Vector3> { }

public class PositionRollback : RollbackBehaviour {
    [SerializeField] RollbackElementVector3 positionRB = new RollbackElementVector3();

    public RollbackElementVector3 GetPositionRB() {
        return positionRB;
    }
    
    public override void Simulate() {
    }

    public override void SetValueFromFrameNumber(int frameNumber) {
        positionRB.SetValueFromFrameNumber(frameNumber);
        transform.position = positionRB.value;
    }

    public override void DeleteFrames(int numFramesToDelete, RollbackManager.DeleteFrameMode deleteFrameMode) {
        positionRB.DeleteFrames(numFramesToDelete, deleteFrameMode);
    }

    public override void SaveFrame() {
        positionRB.SetAndSaveValue(transform.position);
    }
}
}
