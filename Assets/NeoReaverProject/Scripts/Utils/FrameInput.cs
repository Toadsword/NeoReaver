using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameInput {
    private InputActionManager.InputStateEnum shootState_;
    private float horizontalAxisValue_ = 0.0f;
    private float verticalAxisValue_ = 0.0f;

    public FrameInput(InputActionManager.InputStateEnum shootState,
        float horizontalAxisValue,
        float verticalAxisValue) 
    {
        shootState_ = shootState;
        horizontalAxisValue_ = horizontalAxisValue;
        verticalAxisValue_ = verticalAxisValue;
    }
}
