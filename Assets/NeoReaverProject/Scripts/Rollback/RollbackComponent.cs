using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RollbackComponent : MonoBehaviour
{
    
    [SerializeField] public List<string> rollbackedComponentsName = new List<string>();
    [SerializeField] public List<bool> doRollbackComponents = new List<bool>();
    //[SerializeField] public Dictionary<string, bool> rollbackedComponents = new Dictionary<string, bool>();

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

    public void SaveState() {
        foreach (RollbackElement rbElement in elementsToRollback) {
            rbElement.SaveData();
        }
    }

    public void RestoreState(int frameNumber) {

        foreach (RollbackElement rbElement in elementsToRollback) {
            rbElement.Restore(frameNumber);
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
