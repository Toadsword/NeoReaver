using System;
using Packages.EZRollback.Runtime.Scripts;
using UnityEngine;
using UnityEngine.Serialization;

namespace Packages.EZRollback.Runtime.Scripts.RollbackBehaviours {
[Serializable]
public class RollbackElementBool : RollbackElement<bool> { }

public class ActiveRollback : RollbackBehaviour {
    [SerializeField] RollbackElementBool _isActiveRb = new RollbackElementBool();
    
    public override void Simulate() { }

    public override void SetValueFromFrameNumber(int frameNumber) {
        _isActiveRb.SetValueFromFrameNumber(frameNumber);
        gameObject.SetActive(_isActiveRb.value);
    }

    public override void DeleteFrames(int numFramesToDelete, RollbackManager.DeleteFrameMode deleteMode) {
        _isActiveRb.DeleteFrames(numFramesToDelete, deleteMode);
    }

    public override void SaveFrame() {
        _isActiveRb.SetAndSaveValue(gameObject.activeSelf);
    }
}
}