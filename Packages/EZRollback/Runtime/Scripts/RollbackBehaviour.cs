using System;
using UnityEngine;

namespace Packages.EZRollback.Runtime.Scripts {

/**
 * IRollbackBehaviour
 * \brief Class-Interface inherited by MonoBehavior that asks to implement all the necessary functions to include rollback in the script.
 */
[Serializable]
public abstract class RollbackBehaviour : MonoBehaviour {

    /**
     * \brief variable to track if the current IRollbackBehaviour is registered in the RollbackManager or not
     */
    public bool registered = false;
    
    
    public void Awake() {
      RollbackManager.RegisterRollbackBehaviour(this);
    }

    public void OnDestroy() {
      RollbackManager.RegisterRollbackBehaviour(this);
    }
    /*
    public void OnEnable() {
        RollbackManager.RegisterRollbackBehaviour(this);
    }

    public void OnDisable() {
        RollbackManager.UnregisterRollbackBehaviour(this);
    }*/
    
    /**
     * \brief Simulate is called each FixedUpdate(). Execute the code that should be executed each fixed frame.
     * That functions will be called every time a rollback occurs.
     */
    public abstract void Simulate();

    /**
     * \brief Restore a component to a certain frame. Notably used when rewinding in time with rollback.
     * \param frameNumber Num of frame wanted to restore (from 0 to currentFrameNum).
     * Implementation detail : For every RollbackElement<> present in your script, call the SetValueFromFrameNumber function of it.
     * If you are using a special component of Unity, set its new value directly after calling SetValueFromFrameNumber with your RollbackElement<>.value
     */
    public abstract void SetValueFromFrameNumber(int frameNumber);

    /**
     * \brief Delete frames that are no longer useful to the rollback system.
     * \param numFramesToDelete Number of frames to delete at once.
     * \param deleteMode Delete mode of the frames
     * Implementation detail : For every RollbackElement<> present in your script, call the DeleteFrames function of it.
     */
    public abstract void DeleteFrames(int numFramesToDelete,  RollbackManager.DeleteFrameMode deleteMode);

    /**
     * \brief Save the current frame value in the history.
     * Implementation detail : For every RollbackElement<> present in your script, call the SaveFrame function of it.
     */
    public abstract void SaveFrame();
}
}