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
                    case "UnityEngine.SpriteRenderer":
                        SpriteRendererRollback spriteRendererRollback = new SpriteRendererRollback();
                        spriteRendererRollback.Init(gameObject);
                        elementsToRollback.Add(spriteRendererRollback);
                        break;
                    case "PlayerMovement":
                        PlayerMovementRollback playerMovementRollback = new PlayerMovementRollback();
                        playerMovementRollback.Init(gameObject);
                        elementsToRollback.Add(playerMovementRollback);
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

    public void GoToFrame(int frameNumber, bool deleteFrames) {
        foreach (RollbackElement rbElement in elementsToRollback) {
            rbElement.GoToFrame(frameNumber, deleteFrames);
        }
    }
}
