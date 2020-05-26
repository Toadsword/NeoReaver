using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputRecorderFrames : MonoBehaviour {
    public int frameCount = 0;
    public Timer countPerSecond;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    void FixedUpdate() {
        
        if (InputActionManager.GetInputDown(InputActionManager.InputType.SHOOT)) {
            Debug.Log("Shoot : Down, Frame : " + frameCount);
        }
        if (InputActionManager.GetInputUp(InputActionManager.InputType.SHOOT)) {
            Debug.Log("Shoot : Up, Frame : " + frameCount);
        }
        if (InputActionManager.GetInput(InputActionManager.InputType.SHOOT)) {
            Debug.Log("Shoot : Help, Frame : " + frameCount);
        }
        frameCount++;
    }

    void OnGUI() {
        GUI.Label(new Rect(10, 10, 300, 50), (1.0f / Time.deltaTime).ToString() + "fps");
        GUI.Label(new Rect(10, 30, 300, 50), (1.0f / Time.fixedDeltaTime).ToString() + "fixed fps");
    }
}
