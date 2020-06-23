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
        Debug.Log("Inited");
        _refGameObject = gameObject;

        positions = new List<Vector3>();
        rotations = new List<Quaternion>();
        
        lastSavedFrame = 0;
    }
    
    public override void SaveFrame() {
        positions.Add(_refGameObject.transform.position);
        rotations.Add(_refGameObject.transform.rotation);
        lastSavedFrame++;
    }
    
    public override void GoBack(int frameNumber, bool deleteFrames) {
        if (lastSavedFrame < frameNumber) {
            Debug.LogError("Cannot restore from higher number of registered frames");
            return;
        }
        
        _refGameObject.transform.position = positions[frameNumber - 1];
        _refGameObject.transform.rotation = rotations[frameNumber - 1];

        if (deleteFrames) {
            positions.RemoveAt(positions.Count - 1);
            rotations.RemoveAt(rotations.Count - 1);

            lastSavedFrame = frameNumber;
        }
    }

    public override void GoForward(int frameNumber) {
        if (lastSavedFrame < frameNumber) {
            Debug.LogError("Cannot forward from higher number of registered frames");
            return;
        }
        _refGameObject.transform.position = positions[frameNumber - 1];
        _refGameObject.transform.rotation = rotations[frameNumber - 1];
    }
}
