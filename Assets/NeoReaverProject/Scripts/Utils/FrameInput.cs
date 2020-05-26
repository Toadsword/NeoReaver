using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameInput {
    
    // One for each action
    private InputState shootState_;
    
    //One for each axis
    private float horizontalAxisValue_ = 0.0f;
    private float verticalAxisValue_ = 0.0f;

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
