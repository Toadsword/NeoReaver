using System;
using System.Collections.Generic;
using UnityEngine;

namespace Packages.EZRollback.Runtime.Scripts 
{

/**
 * \brief Class to allow serialization of RollbackElement of type RollbackInputBaseActions
 */
[Serializable]
public class RollbackElementRollbackInputBaseActions : RollbackElement<RollbackInputBaseActions> {}

/**
 * \brief Rollback manager that deals with input. Allow registering inputs and rewind them in time.
 */
public abstract class RollbackInputManager : MonoBehaviour {
    public int localPlayerId;

    private int _defaultNumberOfInputs = 1;
    
    public enum AxisEnum {
        HORIZONTAL,
        VERTICAL
    }
    
    [SerializeField] protected List<RollbackElementRollbackInputBaseActions> _playerInputList = new List<RollbackElementRollbackInputBaseActions>();

    void OnEnable() {
        if (_playerInputList == null) {
            _playerInputList = new List<RollbackElementRollbackInputBaseActions>();
        }

        _defaultNumberOfInputs = GetNumberOfInputs();
    }

    protected abstract int GetNumberOfInputs();

    public int GetNumberOfBitsNeeded() {
        return 2 + (_defaultNumberOfInputs / 8);
    }

    public RollbackElementRollbackInputBaseActions GetPlayerInputHistory(int playerId) {
        return _playerInputList[playerId];
    } 
    
    /**
     * \brief Add a player to the list of inputs.
     * \return The number of current players in the list.
     */
    public virtual int AddPlayer() {
        _playerInputList.Add(new RollbackElementRollbackInputBaseActions());
        _playerInputList[_playerInputList.Count - 1].value = new RollbackInputBaseActions(_defaultNumberOfInputs);
        return _playerInputList.Count - 1;
    }
    
    /**
     * \brief Set and save the current value of the player.
     * \param playerId ID of the player to add an input.
     */
    public void AddInput(int playerId) {
        AddInput(playerId, GetCurrentActionsValue(playerId));
    }

    /**
     * \brief Set and save the value of the player.
     * \param playerId ID of the player to add an input.
     * \param rbInputBaseActions inputs to save.
     */
    public void AddInput(int playerId, RollbackInputBaseActions rbInputBaseActions) {
        _playerInputList[playerId].SetAndSaveValue(rbInputBaseActions);
    }

    /**
     * \brief Calculate the current state of the input system. Need to be implemented accordingly to your needs with your used input system.
     * \param playerId ID of the player to add an input.
     */
    protected abstract RollbackInputBaseActions GetCurrentActionsValue(int playerId);

    /**
     * \brief Update the status of the inputs of all registered players.
     */
    public virtual void UpdateInputStatus() {
        for (int i = 0; i < _playerInputList.Count; i++) {
            RollbackInputBaseActions actionsValue = GetCurrentActionsValue(i);

            _playerInputList[i].value = actionsValue;
        }
    }
 
    /**
     * \brief Get the value of the requested axis, of a playerId to a certain frame.
     * This function can be overwritten to include more axis
     * \param axis Axis to retrieve
     * \param playerId Id of the player
     * \param frameNumber Frame number to get. Returns the current value by default
     * \return Value of the axis
     */
    public virtual float GetAxis(AxisEnum axis, int playerId, int frameNumber = -1) {
        if (playerId >= _playerInputList.Count)
            return 0.0f;
        
        frameNumber = CheckFrameNumber(frameNumber);
        switch (axis) {
            case AxisEnum.VERTICAL:
                return _playerInputList[playerId].GetValue(frameNumber).GetVerticalAxis();
            case AxisEnum.HORIZONTAL:
                return _playerInputList[playerId].GetValue(frameNumber).GetHorizontalAxis();
        }

        return 0.0f;
    }
    
    /**
     * \brief Get the value of the button, of a playerId to a certain frame.
     * This function can be overwritten to special needs
     * \param actionValue Action to retrieve
     * \param playerId Id of the player
     * \param frameNumber Frame number to get. Returns the current value by default
     * \return Value of the action, true if pressed, false otherwise
     */
    public virtual bool GetInput(int actionValue, int playerId, int frameNumber = -1) {
        if (playerId >= _playerInputList.Count)
            return false;
        
        frameNumber = CheckFrameNumber(frameNumber);
        return _playerInputList[playerId].GetValue(frameNumber).GetValueBit(actionValue);
    }

    /**
     * \brief Get the value of the button if down at that specific frame, of a playerId to a certain frame.
     * This function can be overwritten to special needs
     * \param actionValue Action to retrieve
     * \param playerId Id of the player
     * \param frameNumber Frame number to get. Returns the current value by default
     * \return Value of the action, true if pressed at requested frame, false otherwise
     */
    public virtual bool GetInputDown(int actionValue, int playerId, int frameNumber = -1) {
        if (playerId >= _playerInputList.Count)
            return false;
        
        frameNumber = CheckFrameNumber(frameNumber);

        bool frameBeforeValue = _playerInputList[playerId].GetValue(frameNumber - 1).GetValueBit(actionValue); // GetInput(actionValue, playerId, frameNumber - 1);
        bool currentFrameValue = _playerInputList[playerId].GetValue(frameNumber).GetValueBit(actionValue); //GetInput(actionValue, playerId, frameNumber);

        return !frameBeforeValue && currentFrameValue;
    }
    
    /**
     * \brief Get the value of the button if up at that specific frame, of a playerId to a certain frame.
     * This function can be overwritten to special needs
     * \param actionValue Action to retrieve
     * \param playerId Id of the player
     * \param frameNumber Frame number to get. Returns the current value by default
     * \return Value of the action, true if released at requested frame, false otherwise
     */
    public virtual bool GetInputUp(int actionValue, int playerId, int frameNumber = -1) {
        if (playerId >= _playerInputList.Count)
            return false;
        
        frameNumber = CheckFrameNumber(frameNumber);
        return _playerInputList[playerId].GetValue(frameNumber - 1).GetValueBit(actionValue) && 
               !_playerInputList[playerId].GetValue(frameNumber).GetValueBit(actionValue);
    }
    
    /**
     * \brief Get the name of the action at requested index. 
     * \param actionIndex 
     * \return String value of the action
     */
    public virtual string GetActionName(int actionIndex) {
        return actionIndex.ToString();
    }

    /**
     * \brief Returns the number of registered players
     * \return 
     */
    public int GetNumOfPlayers() {
        return _playerInputList.Count;
    }

    /**
     * \brief Correct the inputs of a given player for a number of frames from the actual frames.
     * \param playerId ID of the player
     * \param numFrames Number of frames to correct from the currentFrame
     * \param rbInputBaseActions Array of actions that will replace the current ones
     * \return true if corrected, false otherwise
     */
    public bool CorrectInputs(int playerId, int numFrames, RollbackInputBaseActions[] rbInputBaseActions) {
        int currentFrame = RollbackManager.Instance.GetDisplayedFrameNum();
        bool result = true;
        for (int i = 0; i < numFrames; i++) {
            result = _playerInputList[playerId].CorrectValue(rbInputBaseActions[i],currentFrame - numFrames + i);
            if (!result) break;
        }

        return result;
    }
    
    /**
     * \brief Check the frame number, and depending on the input, correct it and returns a value that will not make the program crash because of a random out of range
     * \param frameNumber Requested frame number
     * \return Corrected value
     */
    private int CheckFrameNumber(int frameNumber) {
        if (frameNumber < 0 || frameNumber > RollbackManager.Instance.GetDisplayedFrameNum()) {
            frameNumber = RollbackManager.Instance.GetDisplayedFrameNum();
        }

        return frameNumber;
    }
    
       
    /**
     * \brief Save the inputs of all the players of the current frames.
     */
    public void SaveFrame() {
        foreach (RollbackElementRollbackInputBaseActions element in _playerInputList) {
            element.SaveFrame();
        }
    }
    
    /**
     * \brief Save the inputs of all the players of the current frames.
     */
    public void SetValueFromFrameNumber(int frameNumber) {
        frameNumber = CheckFrameNumber(frameNumber);
        foreach (RollbackElementRollbackInputBaseActions element in _playerInputList) {
            element.SetValueFromFrameNumber(frameNumber);
        }
    }
    
    /**
     * \brief Delete a certain number of frames for all the players
     * \param numFramesToDelete Number of frames to delete
     * \param bool True to delete the x first frames, False to delete the x last frames registered
     */
    public void DeleteFrames(int numFramesToDelete, RollbackManager.DeleteFrameMode deleteMode) {
        foreach (RollbackElementRollbackInputBaseActions element in _playerInputList) {
            element.DeleteFrames(numFramesToDelete, deleteMode);
        }
    }

    public void ClearInputManager() {
        _playerInputList = new List<RollbackElementRollbackInputBaseActions>();
    }
}
}
