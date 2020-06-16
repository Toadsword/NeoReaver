using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FrameInput {
    
    // One for each action
    [SerializeField] private InputState shootState_;
    
    //One for each axis
    [SerializeField] private float horizontalAxisValue_ = 0.0f;
    [SerializeField] private float verticalAxisValue_ = 0.0f;

    public FrameInput(
        InputState shootState,
        float horizontalAxisValue,
        float verticalAxisValue) 
    {
        shootState_ = shootState;
        horizontalAxisValue_ = horizontalAxisValue;
        verticalAxisValue_ = verticalAxisValue;
    }
}
