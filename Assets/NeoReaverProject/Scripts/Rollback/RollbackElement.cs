using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class RollbackElement{
    protected int lastSavedFrame = 0;

    public virtual void Init(GameObject gameObject) {
        Debug.LogError("Init from RollbackElement not implemented");
    }
    
    // Start is called before the first frame update
    public virtual void SaveFrame() {
        Debug.LogError("SaveData from RollbackElement not implemented");
    }

    public virtual void GoBack(int frameNumber, bool deleteFrame) {
        Debug.LogError("GoBack from RollbackElement not implemented");
    }

    public virtual void GoForward(int frameNumber) {
        Debug.LogError("GoForward from RollbackElement not implemented");
    }
}
