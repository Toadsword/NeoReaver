using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RollbackComponent : MonoBehaviour
{
    [SerializeField] public List<string> rollbackedComponentsName = new List<string>();
    [SerializeField] public List<bool> doRollbackComponents = new List<bool>();

    [SerializeField] List<RollbackElement> elementsToRollback;
    
    void Start() {
        elementsToRollback = new List<RollbackElement>();
        
        for(int i = 0; i < rollbackedComponentsName.Count; i++){
            if (doRollbackComponents[i]) {
                switch (rollbackedComponentsName[i]) {
                    case "UnityEngine.Transform":
                        TransformRollback transformRollback = new TransformRollback();
                        transformRollback.Init(gameObject);
                        elementsToRollback.Add(transformRollback);
                        break;
                    case "PlayerMovement":
                        break;
                }
            }
        }
    }

    public void SaveCurrentFrame() {
        foreach (RollbackElement rbElement in elementsToRollback) {
            rbElement.SaveFrame();
        }
    }

    public void GoBackToFrame(int frameNumber, bool deleteFrames) {
        foreach (RollbackElement rbElement in elementsToRollback) {
            rbElement.GoBack(frameNumber, deleteFrames);
        }
    }

    public void GoForward(int frameNumber) {
        foreach (RollbackElement rbElement in elementsToRollback) {
            rbElement.GoForward(frameNumber);
        }
    }

    public void SetActivenessComponents(bool setActive) {
        for(int i = 0; i < rollbackedComponentsName.Count; i++){
            if (doRollbackComponents[i]) {
                switch (rollbackedComponentsName[i]) {
                    case "PlayerMovement":
                        gameObject.GetComponent<PlayerMovement>().enabled = setActive;
                        break;
                }
            }
        }
    }
}
