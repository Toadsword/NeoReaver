using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollbackManager : MonoBehaviour {
    [SerializeField] bool doRollback = false;
    
    [SerializeField] RollbackComponent[] rollbackElements;
    
    int currentFrameNum = 0;
    
    // Start is called before the first frame update
    void Start() {
        rollbackElements = GameObject.FindObjectsOfType<RollbackComponent>();
        currentFrameNum = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (doRollback) {
            if (currentFrameNum == 0)
                return; 
            
            foreach(RollbackComponent rollbackElement in rollbackElements)
            {
                rollbackElement.RestoreState(currentFrameNum);
                rollbackElement.SetActivenessComponents(false);
            } 
            currentFrameNum--;
        } else {
            foreach(RollbackComponent rollbackElement in rollbackElements)
            {
                rollbackElement.SaveState();
                rollbackElement.SetActivenessComponents(true);
            }
            currentFrameNum++;
        }
    }
}
