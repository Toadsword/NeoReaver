using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[Serializable]
public class RollbackInformation {
    public List<GameObject> objectsToRollback;
    public List<int> instancesIdToRollback;

    public RollbackInformation() {
        objectsToRollback = new List<GameObject>();
        instancesIdToRollback = new List<int>();
    }

    public void Resize(int newSize) {
        while (newSize < objectsToRollback.Count) {
            objectsToRollback.RemoveAt(objectsToRollback.Count - 1);
            instancesIdToRollback.RemoveAt(instancesIdToRollback.Count - 1);
        }

        while (newSize > objectsToRollback.Count) {
            objectsToRollback.Add(null);
            instancesIdToRollback.Add(0);
        }
    }
    
    public void Clear() {
        objectsToRollback.Clear();
        instancesIdToRollback.Clear();
    }

    public bool Contains(int instanceId) {
        return instancesIdToRollback.Contains(instanceId);
    }

    public void Add(GameObject gameObject) {
        objectsToRollback.Add(gameObject);
        if(gameObject == null){
            instancesIdToRollback.Add(0);        
        } else{
            instancesIdToRollback.Add(gameObject.GetInstanceID());
        }
    }
    public void RemoveAt(int i) {
        if (i > objectsToRollback.Count) 
            return;
        
        objectsToRollback.RemoveAt(i);
        instancesIdToRollback.RemoveAt(i);
    }

    public int GetCount() {
        if(objectsToRollback.Count != instancesIdToRollback.Count)
            Resize(objectsToRollback.Count);
        
        return objectsToRollback.Count;
    }
}
