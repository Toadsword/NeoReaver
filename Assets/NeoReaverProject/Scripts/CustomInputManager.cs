using System.Collections;
using System.Collections.Generic;
using Packages.EZRollback.Runtime.Scripts;
using UnityEngine;

public class CustomInputManager : RollbackInputManager
{
    public enum ActionsCode : int{
        UP = 0,
        RIGHT = 1,
        DOWN = 2,
        LEFT = 3,
        
        SHOOT = 4,
        
        LENGTH
    }
    
    protected  override RollbackInputBaseActions GetCurrentActionsValue(int controllerId) {
        if (controllerId == Logic.localPlayerId) {
            //Gather local inputs and execute them
            RollbackInputBaseActions actionsValue = new RollbackInputBaseActions((int)ActionsCode.LENGTH);
            
            SetBitFromKeycode((int)ActionsCode.UP, KeyCode.W, ref actionsValue);
            SetBitFromKeycode((int)ActionsCode.RIGHT, KeyCode.D, ref actionsValue);
            SetBitFromKeycode((int)ActionsCode.DOWN, KeyCode.S, ref actionsValue);
            SetBitFromKeycode((int)ActionsCode.LEFT, KeyCode.A, ref actionsValue);
            actionsValue.SetOrClearBit((int)ActionsCode.SHOOT, Input.GetMouseButton(1));

            actionsValue.SetHorizontalAxis(Input.GetAxisRaw("Horizontal"));
            actionsValue.SetVerticalAxis(Input.GetAxisRaw("Vertical"));

            return actionsValue;
        } else {
            //Predict the next input by copying the last input
            return _playerInputList[controllerId].value;
        }
    }

    public override int AddPlayer() {
        Debug.Log("AddPlayer");
        return base.AddPlayer();
    }

    void SetBitFromKeycode(int inputIndex, KeyCode keyCode, ref RollbackInputBaseActions actionsValue) {
        actionsValue.SetOrClearBit(inputIndex, Input.GetKey(keyCode));
    }

    public override string GetActionName(int actionIndex) {
        switch (actionIndex) {
            case 0 : return "Up";
            case 1 : return "Right";
            case 2 : return "Down";
            case 3 : return "Left";
            case 4 : return "Left mouse button";
        }

        return "out";
    }
}