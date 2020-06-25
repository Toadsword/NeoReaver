using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TransformRollback : RollbackElement {

    public List<Vector3> positions;
    public List<Quaternion> rotations;

    GameObject _refGameObject;
    
    public override void Init(GameObject gameObject) {
        _refGameObject = gameObject;

        positions = new List<Vector3>();
        rotations = new List<Quaternion>();
    }
    
    public override void SaveFrame() {
        positions.Add(_refGameObject.transform.position);
        rotations.Add(_refGameObject.transform.rotation);
        totalSavedFrame++;
    }
    
    protected override void GoToFrame(int frameNumber) {
        _refGameObject.transform.position = positions[frameNumber - 1];
        _refGameObject.transform.rotation = rotations[frameNumber - 1];
    }

    protected override void DeleteFrame(int frameNumber) {
        positions.RemoveAt(positions.Count - 1);
        rotations.RemoveAt(rotations.Count - 1);
    }
}
