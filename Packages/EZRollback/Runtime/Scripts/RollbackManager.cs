using System;
using Packages.EZRollback.Runtime.Scripts.Utils;
using UnityEngine;

namespace Packages.EZRollback.Runtime.Scripts {
    /**
     * \brief The RollbackManager is main rollback system present in the scene. It is required to allow your scripts to rewind in time.
     */
    public class RollbackManager : Singleton<RollbackManager> {

    public enum DeleteFrameMode {
        FIRST_FRAMES,
        LAST_FRAMES,
        FIT_TO_BUFFER
    }
    
    public bool registerFrames = false;
    public bool bufferRestriction = false;
    
    /** ----------------- STATICS -------------------- **/
    /**
     * Delegates are created to make a callback to all registered functions when rewinding in time or going forward.
     */
    public Action prepareInputDelegate;
    
    //Callbacks
    public static Action simulateDelegate;
    public static Action saveDelegate;
    public static Action saveInputDelegate;
    public static Action<int> goToFrameDelegate;
    public static Action<int> goToFrameInputDelegate;
    public static Action<int, DeleteFrameMode> deleteFramesDelegate;
    public static Action<int, DeleteFrameMode> deleteFramesInputDelegate;

    public static RollbackInputManager rbInputManager;
    
    [SerializeField] int _maxFrameNum = 0;
    [SerializeField] int _displayedFrameNum = 0;
    [SerializeField] int _indexFrameNumFromStart = 0;

    [SerializeField] public int bufferSize = -1;

    /* ----- Getter and Setters ------ */
    public RollbackInputManager GetRBInputManager() {
        return rbInputManager;
    }

    public int GetIndexFrameNumFromStart() {
        return _indexFrameNumFromStart;
    }
    
    public int GetDisplayedFrameNum() {
        return _displayedFrameNum;
    }

    public int GetMaxFramesNum() {
        return _maxFrameNum;
    }

    RollbackBehaviour[] _rbRegisteredBehaviours;
    void OnEnable() {
        ResetRbInputManagerEvents(false);
    }
    void OnDisable() {
        //Unregister the inputs callbacks
        ResetRbInputManagerEvents(true);    
    }

    public void ResetRbInputManagerEvents(bool onlyUnset = false) {
        if (rbInputManager != null) {
            prepareInputDelegate -= rbInputManager.UpdateInputStatus;
            saveInputDelegate -= rbInputManager.SaveFrame;
            goToFrameInputDelegate -= rbInputManager.SetValueFromFrameNumber;
            deleteFramesInputDelegate -= rbInputManager.DeleteFrames;
            rbInputManager = null;
        }

        if (onlyUnset) 
            return;
        
        rbInputManager = GetComponent<RollbackInputManager>();
        if (rbInputManager == null)
            return;
        
        prepareInputDelegate += rbInputManager.UpdateInputStatus;
        saveInputDelegate += rbInputManager.SaveFrame;
        goToFrameInputDelegate += rbInputManager.SetValueFromFrameNumber;
        deleteFramesInputDelegate += rbInputManager.DeleteFrames;
    }

    /**
     * \brief Register an RollbackBehaviour to the manager's rollback callback
     * \param rbBehaviour RollbackBehaviour to register
     */
    public static void RegisterRollbackBehaviour(RollbackBehaviour rbBehaviour) {
        if (rbBehaviour.registered)
            return;
        
        simulateDelegate += rbBehaviour.Simulate;
        saveDelegate += rbBehaviour.SaveFrame;
        goToFrameDelegate += rbBehaviour.SetValueFromFrameNumber;
        deleteFramesDelegate += rbBehaviour.DeleteFrames;
        
        rbBehaviour.registered = true;
    }
    
    /**
     * \brief Unregister an RollbackBehaviour from the manager's rollback callback.
     * \param rbBehaviour RollbackBehaviour to unregister
     */
    public static void UnregisterRollbackBehaviour(RollbackBehaviour rbBehaviour) {
        if (!rbBehaviour.registered)
            return;
        
        simulateDelegate -= rbBehaviour.Simulate;
        saveDelegate -= rbBehaviour.SaveFrame;
        goToFrameDelegate -= rbBehaviour.SetValueFromFrameNumber;
        deleteFramesDelegate -= rbBehaviour.DeleteFrames;
        
        rbBehaviour.registered = false;
    }

    void Start() {
        _indexFrameNumFromStart = 0;
        _displayedFrameNum = 0;
        _maxFrameNum = 0;
    }

    void FixedUpdate() {
        if(deleteFramesDelegate == null)
            return;
        
        if(registerFrames) {
            Simulate(1);
        }
    }

    public void ClearRollbackManager() {
        deleteFramesDelegate?.Invoke(GetMaxFramesNum(), DeleteFrameMode.FIRST_FRAMES);
        deleteFramesInputDelegate?.Invoke(GetMaxFramesNum(), DeleteFrameMode.FIRST_FRAMES);
        
        rbInputManager.ClearInputManager();
    }

    /**
     * \brief Setting the current frame as last registered, and delete all future frames currently registered
     * \param deleteInputs Also delete input frames if true, doesn't otherwise
     */
    private void SetCurrentFrameAsLastRegistered(bool deleteInputs = true) {
        if (_displayedFrameNum == _maxFrameNum) 
            return;
        
        //Apply set
        deleteFramesDelegate?.Invoke(_maxFrameNum - _displayedFrameNum, DeleteFrameMode.LAST_FRAMES);
        if (deleteInputs) {
            deleteFramesInputDelegate?.Invoke(_maxFrameNum - _displayedFrameNum, DeleteFrameMode.LAST_FRAMES);
        }
        _indexFrameNumFromStart -= (_maxFrameNum - _displayedFrameNum);
        _maxFrameNum = _displayedFrameNum;
    }

    /**
     * \brief Get back in x frames
     * \param numFrames Number of frames to rollback
     * \param deleteFrames true if we want to delete the frames we rewind.
     * \param inputsToo along with deleting frames, delete the input frames if true.
     */
    public void GoBackInFrames(int numFrames, bool deleteFrames = true, bool inputsToo = true) {
        SetValueFromFrameNumber(GetDisplayedFrameNum() - numFrames, deleteFrames, inputsToo);
    }
    
    /**
     * \brief Get back to a specific frame number
     * \param frameNumber Frame number wanted
     * \param deleteFrames true if we want to delete the frames we rewind.
     * \param inputsToo along with deleting frames, delete the input frames if true.
     */
    public void SetValueFromFrameNumber(int frameNumber, bool deleteFrames = true, bool inputsToo = true) {
        if (_maxFrameNum < frameNumber || frameNumber < 0)
            return;

        //Apply Goto
        goToFrameInputDelegate?.Invoke(frameNumber);
        goToFrameDelegate?.Invoke(frameNumber);

        _displayedFrameNum = frameNumber;
        if (deleteFrames) {
            SetCurrentFrameAsLastRegistered(inputsToo);
            _maxFrameNum = _displayedFrameNum;
        }
    }

    /**
     * \brief Save the value of the current frame
     * \param inputsToo True to save the current state of the inputs, false otherwise. False is used when rewinding frames with calculated new inputs
     */
    public void SaveCurrentFrame(bool inputsToo = true) {
        //If we try to save a frame while in restored state, we delete the first predicted future
        SetCurrentFrameAsLastRegistered(inputsToo);

        //Apply save
        saveDelegate?.Invoke();
        if (inputsToo) {
            saveInputDelegate?.Invoke();
        }
        
        _displayedFrameNum++;
        _indexFrameNumFromStart++;
        _maxFrameNum = _displayedFrameNum;
    }

    /**
     * \brief Simulate a certain number of frames
     * \param numFrames Number of frames to simulate
     * \param inputsToo True to save the current state of the inputs, false otherwise. False is used when rewinding frames with calculated new inputs
     */
    public void Simulate(int numFrames, bool inputsToo = true) {
        SetCurrentFrameAsLastRegistered(inputsToo);

        for (int i = 0; i < numFrames; i++) {
            //Apply simulate and save for each frames
            if (inputsToo){
                prepareInputDelegate?.Invoke();
            } else {
                goToFrameInputDelegate?.Invoke(_displayedFrameNum + 1);
            }
            simulateDelegate?.Invoke();
            SaveCurrentFrame(inputsToo);
        }
        
        if (bufferRestriction && inputsToo) {
            ManageBufferSize();           
        }
    }
    
    /**
     * \brief Simulate a certain number of frames, while taking in count the already defined inputs of the players
     * \param numFrames Number of frames to resimulate
     */
    public void ReSimulate(int numFrames) {
        if (numFrames >= _maxFrameNum) {
            numFrames = _maxFrameNum - 1;
        }
        
        GoBackInFrames(numFrames, true, false);
        Simulate(numFrames, false);
    }

    /**
     * \brief Resize the frame buffer if it exceeds its size
     */
    private void ManageBufferSize() {
        if (bufferSize > 0 && _maxFrameNum > bufferSize) {
            deleteFramesDelegate?.Invoke(1, DeleteFrameMode.FIT_TO_BUFFER);
            deleteFramesInputDelegate?.Invoke(1, DeleteFrameMode.FIT_TO_BUFFER);

            _maxFrameNum = bufferSize;
            _displayedFrameNum = _maxFrameNum;
        }
    }
}
}
