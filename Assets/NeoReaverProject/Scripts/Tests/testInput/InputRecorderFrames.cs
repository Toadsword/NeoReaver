using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputRecorderFrames : MonoBehaviour {
    public int frameCount = 0;

    public float deadZone = 0.1f;
    
    InputState shootInputState_;

    List<FrameInput> LocalFrameInputs;

    float horizontalInput_;
    float verticalInput_;
    
    // Update is called once per frame
    void Update() {
        ComputeUpdateInputState(shootInputState_, InputActionManager.InputType.SHOOT);

        float horizontalInput = InputActionManager.GetAxis(InputActionManager.AxisType.HORIZONTAL);
        if (Math.Abs(horizontalInput) > deadZone && 
            Math.Abs(horizontalInput) >= Math.Abs(horizontalInput_)) 
        {
            horizontalInput_ = horizontalInput;
        }
        
        float verticalInput = InputActionManager.GetAxis(InputActionManager.AxisType.HORIZONTAL);
        if (Math.Abs(verticalInput) > deadZone && 
            Math.Abs(verticalInput) >= Math.Abs(verticalInput_)) 
        {
            verticalInput_ = verticalInput;
        }
        
    }

    void FixedUpdate() {
        ResetInputs();

        RegisterNewInputFrame();
        
        LocalFrameInputs.Add(new FrameInput(shootInputState_, horizontalInput_, verticalInput_));
        
        frameCount++;
    }

    void ComputeUpdateInputState(InputState inputState, InputActionManager.InputType action) {
        if (InputActionManager.GetInputDown(action)) {
            inputState.isDown = true;
            inputState.isHeld = true;
        }

        if (InputActionManager.GetInputUp(action)) {
            inputState.isHeld = false;
            inputState.isUp = true;
        }
    }
    
    void ResetInputs() {
        shootInputState_.Reset();
        
        horizontalInput_ = 0.0f;
        verticalInput_ = 0.0f;
    }

    void RegisterNewInputFrame() {
        
    }

    void OnGUI() {
        GUI.Label(new Rect(10, 10, 300, 50), (1.0f / Time.deltaTime).ToString() + "fps");
        GUI.Label(new Rect(10, 30, 300, 50), (1.0f / Time.fixedDeltaTime).ToString() + "fixed fps");
    }
}
