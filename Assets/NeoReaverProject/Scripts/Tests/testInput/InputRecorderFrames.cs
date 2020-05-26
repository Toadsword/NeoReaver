using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputRecorderFrames : MonoBehaviour {
    public int frameCount = 0;
    public Timer countPerSecond;

    bool isDown = false;
    bool isHeld = false;
    bool isUp = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (InputActionManager.GetInputDown(InputActionManager.InputType.SHOOT)) {
            isDown = true;
            isHeld = true;
        }
        if (InputActionManager.GetInputUp(InputActionManager.InputType.SHOOT)) {
            isHeld = false;
            isUp = true;
            //  Debug.Log("Shoot : Up, Frame : " + frameCount);
        }
    }

    void FixedUpdate() {
        if (isDown) {
            Debug.Log("Shoot : Down, Frame : " + frameCount);
            isDown = false;
        }

        if (isHeld) {
            Debug.Log("Shoot : Held, Frame : " + frameCount);
        }

        if (isUp) {
            Debug.Log("Shoot : Up, Frame : " + frameCount);
            isUp = false;
        }
        frameCount++;
    }

    void OnGUI() {
        GUI.Label(new Rect(10, 10, 300, 50), (1.0f / Time.deltaTime).ToString() + "fps");
        GUI.Label(new Rect(10, 30, 300, 50), (1.0f / Time.fixedDeltaTime).ToString() + "fixed fps");
    }
}
