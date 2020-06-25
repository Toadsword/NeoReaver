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
            GoToFrame(currentFrameNum - 1);
        } else {
            SaveCurrentFrame();
        }
    }

    public void GoToFrame(int frameNumber, bool deleteFrames = true) {
        if (maxFrameNum < frameNumber)
            return;
        
        foreach(RollbackComponent rollbackElement in rollbackElements)
        {
            rollbackElement.GoToFrame(currentFrameNum, deleteFrames);
        }

        currentFrameNum = frameNumber;
        if (deleteFrames) {
            maxFrameNum = currentFrameNum;
        }
    }
    
    
    public void SaveCurrentFrame() {
        //If we try to save a frame while in restored state, we delete the first predicted future
        if (currentFrameNum != maxFrameNum) {
            foreach(RollbackComponent rollbackElement in rollbackElements) {
                rollbackElement.GoToFrame(currentFrameNum, true);
            }
        }
        
        foreach(RollbackComponent rollbackElement in rollbackElements)
        {
            rollbackElement.SaveCurrentFrame();
        }
        currentFrameNum++;
        maxFrameNum = currentFrameNum;
    }
}
