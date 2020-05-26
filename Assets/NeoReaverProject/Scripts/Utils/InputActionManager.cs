using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public static class InputActionManager {

    static bool logInput = false;

    static float deadZoneKeyboard = 0.1f;
    static float minDeltaMove = 0.0001f;
    public static bool keyboardLastPressed = false;

    public enum InputType
    {
        LEFT, // Directions like those are useful for the navigation in the menu
        RIGHT,
        UP,
        DOWN,
        ACTION_CONFIRM,
        ACTION_BACK,
        
        SHOOT,

        PAUSE
    }
    public enum AxisType
    {
        HORIZONTAL,
        VERTICAL,
    }
    public enum InputStateEnum
    {
        UP,
        DOWN,
        HOLD
    }
    public enum DirectionType
    {
        R_INPUT,
        L_INPUT
    }

    public enum MouseButton {
        LEFT,
        RIGHT
    }

    /* public functions, callable on any scripts */
    public static bool GetInputDown(InputType inputType, InputDevice inputDevice = null)
    {
        return GetInput(inputType, InputStateEnum.DOWN, inputDevice);
    }
    public static bool GetInputUp(InputType inputType, InputDevice inputDevice = null)
    {
        return GetInput(inputType, InputStateEnum.UP, inputDevice);
    }
    public static bool GetInput(InputType inputType, InputDevice inputDevice = null)
    {
        return GetInput(inputType, InputStateEnum.HOLD, inputDevice);
    }

    /* Call input principal function
     *
     * To change configuration of controller or keyboard, do it here !    
     */
    private static bool GetInput(InputType inputType, InputStateEnum timeType, InputDevice inputDevice = null)
    {
        InputDevice device = InputManager.ActiveDevice;
        if (inputDevice != null)
            device = inputDevice;

        bool result = false;
        switch (inputType)
        {
            case InputType.LEFT:
                result = GetInput(InputControlType.LeftStickLeft, timeType, device) ||
                         GetInput(InputControlType.DPadLeft, timeType, device) ||
                         GetInput(InputControlType.RightStickLeft, timeType, device) ||
                         GetInput(KeyCode.RightArrow, timeType);
                break;
            case InputType.RIGHT:
                result = GetInput(InputControlType.LeftStickRight, timeType, device) ||
                         GetInput(InputControlType.DPadRight, timeType, device) ||
                         GetInput(InputControlType.RightStickRight, timeType, device) ||
                         GetInput(KeyCode.LeftArrow, timeType);
                break;
            case InputType.UP:
                result = GetInput(InputControlType.LeftStickUp, timeType, device) ||
                         GetInput(InputControlType.DPadUp, timeType, device) ||
                         GetInput(InputControlType.RightStickUp, timeType, device) ||
                         GetInput(KeyCode.UpArrow, timeType);
                break;
            case InputType.DOWN:
                result = GetInput(InputControlType.LeftStickDown, timeType, device) ||
                         GetInput(InputControlType.DPadDown, timeType, device) ||
                         GetInput(InputControlType.RightStickDown, timeType, device) ||
                         GetInput(KeyCode.DownArrow, timeType);
                break;
            case InputType.ACTION_CONFIRM:
                result = GetInput(InputControlType.Action1, timeType) ;
                break;
            case InputType.ACTION_BACK:
                result = GetInput(InputControlType.Action2, timeType) ||
                         GetInput(KeyCode.Escape, timeType);
                break;
            case InputType.SHOOT:
                result = GetInput((int)MouseButton.RIGHT, timeType);
                break;
            case InputType.PAUSE:
                result = GetInput(InputControlType.Start, timeType, device) ||
                         GetInput(InputControlType.Menu, timeType, device) ||
                         GetInput(KeyCode.Escape, timeType);
                break;
        }

        if (logInput && result)
            Debug.Log("Input : " + inputType.ToString() + " was pressed and called !");

        CheckControllerLastUsed();
        return result;
    }

    private static bool GetInput(InputControlType input, InputStateEnum timeType, InputDevice inputDevice = null)
    {
        InputDevice device = InputManager.ActiveDevice;
        if (inputDevice != null)
            device = inputDevice;

        switch (timeType)
        {
            case InputStateEnum.UP:
                return device.GetControl(input).WasReleased;
            case InputStateEnum.DOWN:
                return device.GetControl(input).WasPressed;
            case InputStateEnum.HOLD:
                return device.GetControl(input).IsPressed;
        }
        return false;
    }
    private static bool GetInput(KeyCode keyCode, InputStateEnum timeType)
    {
        switch (timeType)
        {
            case InputStateEnum.UP:
                return Input.GetKeyUp(keyCode);
            case InputStateEnum.DOWN:
                return Input.GetKeyDown(keyCode);
            case InputStateEnum.HOLD:
                return Input.GetKey(keyCode);
        }
        return false;
    }
    private static bool GetInput(int mouseBtn, InputStateEnum timeType)
    {
        switch (timeType)
        {
            case InputStateEnum.UP:
                return Input.GetMouseButtonUp(mouseBtn);
            case InputStateEnum.DOWN:
                return Input.GetMouseButtonDown(mouseBtn);
            case InputStateEnum.HOLD:
                return Input.GetMouseButton(mouseBtn);
        }
        return false;
    }

    public static float GetAxis(AxisType axisType, InputDevice inputDevice = null)
    {
        float deltaMove = 0.0f;

        InputDevice device = InputManager.ActiveDevice;
        if (inputDevice != null)
            device = inputDevice;

        switch (axisType)
        {
            case AxisType.HORIZONTAL:
                if (device.GetControl(InputControlType.LeftStickX).IsPressed)
                    deltaMove = device.GetControl(InputControlType.LeftStickX).Value;
                else if (device.GetControl(InputControlType.DPadX).IsPressed)
                    deltaMove = device.GetControl(InputControlType.DPadX).Value;
                else if(Mathf.Abs(Input.GetAxisRaw("Horizontal")) > deadZoneKeyboard)
                    deltaMove = Input.GetAxisRaw("Horizontal");
                break;
            case AxisType.VERTICAL:
                if (device.GetControl(InputControlType.LeftStickY).IsPressed)
                    deltaMove = device.GetControl(InputControlType.LeftStickY).Value;
                else if (device.GetControl(InputControlType.DPadY).IsPressed)
                    deltaMove = device.GetControl(InputControlType.DPadY).Value;
                else if(Mathf.Abs(Input.GetAxisRaw("Vertical")) > deadZoneKeyboard)
                    deltaMove = Input.GetAxisRaw("Vertical");
                break;
        }
        if (logInput && Math.Abs(deltaMove) > minDeltaMove)
            Debug.Log("Moving : " + axisType.ToString() + " " + deltaMove);

        CheckControllerLastUsed();
        return deltaMove;
    }

    public static float GetAxisRaw(AxisType axisType, InputDevice inputDevice = null)
    {
        float deltaMove = 0.0f;

        InputDevice device = InputManager.ActiveDevice;
        if (inputDevice != null)
            device = inputDevice;

        switch (axisType)
        {
            case AxisType.HORIZONTAL:
                if (device.GetControl(InputControlType.LeftStickX).IsPressed)
                    deltaMove = device.GetControl(InputControlType.LeftStickX).RawValue;
                else if (device.GetControl(InputControlType.DPadX).IsPressed)
                    deltaMove = device.GetControl(InputControlType.DPadX).RawValue;
                break;
            case AxisType.VERTICAL:
                if (device.GetControl(InputControlType.LeftStickY).IsPressed)
                    deltaMove = device.GetControl(InputControlType.LeftStickY).RawValue;
                else if (device.GetControl(InputControlType.DPadY).IsPressed)
                    deltaMove = device.GetControl(InputControlType.DPadY).RawValue;
                break;
        }
        if (logInput && Math.Abs(deltaMove) > minDeltaMove)
            Debug.Log("Moving : " + axisType.ToString() + " " + deltaMove);

        CheckControllerLastUsed();
        return deltaMove;
    }

    // Asks for the screenPosition if using the keyboard
    public static Vector2 GetDirection(DirectionType directionType, Vector2 origin, InputDevice inputDevice = null)
    {
        Vector2 direction = new Vector2(0, 0);
        CheckControllerLastUsed();

        if(directionType == DirectionType.R_INPUT && keyboardLastPressed)
        {
            Vector3 v3 = Input.mousePosition;
            v3.z = 10.0f;
            //v3 = Camera.main.ScreenToWorldPoint(v3);

            direction = new Vector2(v3.x - origin.x, v3.y - origin.y);
        }
        else
        {
            direction.x = GetAxis(AxisType.HORIZONTAL, inputDevice);
            direction.y = GetAxis(AxisType.VERTICAL, inputDevice);
        }
        
        direction.Normalize();

        if (logInput && (Math.Abs(direction.x) > minDeltaMove || Math.Abs(direction.y) > minDeltaMove))
            Debug.Log("Direction : " + direction.ToString());

        return direction;
    }

    private static void CheckControllerLastUsed()
    {
        InputDevice device = InputManager.ActiveDevice;
        if (Input.anyKeyDown || Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            keyboardLastPressed = true;
        }
        if (device.AnyButtonWasPressed || 
            device.AnyButtonIsPressed || 
            device.AnyButtonWasReleased ||
            device.LeftBumper.IsPressed ||
            device.RightBumper.IsPressed ||
            device.LeftTrigger.IsPressed ||
            device.RightTrigger.IsPressed)
        {
            keyboardLastPressed = false;
        }
    }
}
