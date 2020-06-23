using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollbackManager : MonoBehaviour {
    public bool doRollback = false;
    
    [SerializeField] RollbackComponent[] rollbackElements;
    int maxFrameNum = 0;
    int currentFrameNum = 0;

    public int GetCurrentFrameNum() {
        return currentFrameNum;
    }

    public int GetMaxFramesNum() {
        return maxFrameNum;
    }
    
    // Start is called before the first frame update
    void Start() {
        rollbackElements = GameObject.FindObjectsOfType<RollbackComponent>();
        currentFrameNum = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (doRollback) {
            Rollback(1);
        } else {
            SaveCurrentFrame();
        }
    }

    public void Rollback(int frameNumber, bool deleteFrames = true) {
        if (currentFrameNum < frameNumber)
            return;
        
        int numFrames = Mathf.Abs(frameNumber - currentFrameNum);
        for (int i = 0; i < numFrames; i++) {
            if (currentFrameNum == 0)
                return; 
            
            foreach(RollbackComponent rollbackElement in rollbackElements)
            {
                rollbackElement.GoBackToFrame(currentFrameNum, deleteFrames);
                rollbackElement.SetActivenessComponents(false);
            } 
            currentFrameNum--;
        }
    }

    public void GoForward(int frameNumber) {
        int numFrames = Mathf.Abs(frameNumber - currentFrameNum);
        for (int i = 0; i < numFrames; i++) {
            currentFrameNum++;
            foreach(RollbackComponent rollbackElement in rollbackElements)
            {
                rollbackElement.GoForward(currentFrameNum);
                rollbackElement.SetActivenessComponents(false);
            } 
        }
    }
    
    public void SaveCurrentFrame() {
        foreach(RollbackComponent rollbackElement in rollbackElements)
        {
            rollbackElement.SaveCurrentFrame();
            rollbackElement.SetActivenessComponents(true);
        }
        currentFrameNum++;
        maxFrameNum = currentFrameNum;
    }
}
