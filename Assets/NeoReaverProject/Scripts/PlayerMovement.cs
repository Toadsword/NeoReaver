using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;
using Packages.EZRollback.Runtime.Scripts;
    

[Serializable]
public struct SpeedValues {
    public float currentSpeedo;
    public float currentSpeedMultiplier;
    public Vector2 direction;
}

[Serializable]
public class RollbackElementSpeedValues : RollbackElement<SpeedValues> { }

public class PlayerMovement : IRollbackBehaviour {

    [Serializable]
    enum MovementState {
        IDLE,
        MOVING,
        CHANGING_DIRECTION
    }

    [SerializeField] bool movable = false;
    
    [SerializeField] float maxSpeedo = 5.0f;
    [SerializeField] float minSpeedo = 0.2f;
    [SerializeField] float speedMultiplier;

    Vector3 fixedLastUpdatePosition;
    [SerializeField] float midSpeedo;
    [SerializeField] MovementState currentMovementState;
    
    [SerializeField] public RollbackElementSpeedValues rbElements = new RollbackElementSpeedValues();
    
    void Start() {
        rbElements.value.currentSpeedo = 5.0f;
        rbElements.value.currentSpeedMultiplier = speedMultiplier;
        
        midSpeedo = (maxSpeedo + minSpeedo) / 2.0f;
        
        RollbackManager.RegisterRollbackBehaviour(this);
    }

    void OnDestroy() {
        RollbackManager.UnregisterRollbackBehaviour(this);
    }

    private void MoveSpaceship(Vector3 initPosition) {

        if (!movable) {
            return;
        }
        
        float angle = Mathf.Atan2(rbElements.value.direction.y, rbElements.value.direction.x) * Mathf.Rad2Deg - 90.0f;
        float currentAngle = (transform.rotation.eulerAngles.z + 90.0f) * Mathf.Deg2Rad;
        Quaternion newRotation = transform.rotation;
        if (rbElements.value.direction != Vector2.zero) 
        {
            newRotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * 450.0f);
            // Calculating new state
            if (transform.rotation != newRotation) {
                currentMovementState = MovementState.CHANGING_DIRECTION;
            } else {
                currentMovementState = MovementState.MOVING;
            }
        } else {
            currentMovementState = MovementState.IDLE;
        }
        
        transform.rotation = newRotation;
        float checkingMaxSpeedo = maxSpeedo;
        switch (currentMovementState) {
            case MovementState.IDLE:
                rbElements.value.currentSpeedMultiplier = speedMultiplier;
                rbElements.value.currentSpeedo /= rbElements.value.currentSpeedMultiplier;
                break;
            case MovementState.MOVING:
                rbElements.value.currentSpeedo *= rbElements.value.currentSpeedMultiplier * 5.0f;
                break;
            case MovementState.CHANGING_DIRECTION:
                if (currentAngle - angle > 60.0f) {
                    rbElements.value.currentSpeedMultiplier = speedMultiplier;
                } else {
                    rbElements.value.currentSpeedo *= rbElements.value.currentSpeedMultiplier;
                } 
                break;
        }
            
        if (rbElements.value.currentSpeedo > checkingMaxSpeedo) {
            rbElements.value.currentSpeedo = checkingMaxSpeedo;
        }
        if (rbElements.value.currentSpeedo < minSpeedo) {
            rbElements.value.currentSpeedo = minSpeedo;
        }

        Vector3 currentDirection = new Vector3(Mathf.Cos(currentAngle) , Mathf.Sin(currentAngle), 0.0f);
        transform.position = initPosition + currentDirection * rbElements.value.currentSpeedo * Time.fixedDeltaTime;
    }

    public override void Simulate() {
        MoveSpaceship(transform.position);
    }

    public override void GoToFrame(int frameNumber) {
        rbElements.SetValueFromFrameNumber(frameNumber);
    }

    public override void DeleteFrames(int numFramesToDelete, bool fromFrames) {
        rbElements.DeleteFrames(numFramesToDelete, fromFrames);
    }

    public override void SaveFrame() {
        rbElements.SaveFrame();
    }
}
