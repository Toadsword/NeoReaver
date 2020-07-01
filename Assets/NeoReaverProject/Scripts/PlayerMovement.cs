using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class PlayerMovement : IRollbackBehaviour {

    [Serializable]
    enum MovementState {
        IDLE,
        MOVING,
        CHANGING_DIRECTION
    }
    
    [SerializeField] float maxSpeedo = 5.0f;
    [SerializeField] float minSpeedo = 0.2f;
    [SerializeField] float speedMultiplier;

    Vector3 fixedLastUpdatePosition;
    [SerializeField] float midSpeedo;
    [SerializeField] RollbackElement<float> currentSpeedo = new RollbackElement<float>();
    [SerializeField] MovementState currentMovementState;

    //Rollback elements
    public RollbackElement<Vector2> direction = new RollbackElement<Vector2>();
    [SerializeField] RollbackElement<float> currentSpeedMultiplier = new RollbackElement<float>();
    private RollbackElement<Vector3> positionRB = new RollbackElement<Vector3>();
    private RollbackElement<Quaternion> rotationRB = new RollbackElement<Quaternion>();
    
    void Start() {
        currentSpeedo.value = 5.0f;
        midSpeedo = (maxSpeedo + minSpeedo) / 2.0f;
        currentSpeedMultiplier.value = speedMultiplier;
    }

    private void MoveSpaceship(Vector3 initPosition, float deltaTime) {

        float angle = Mathf.Atan2(direction.value.y, direction.value.x) * Mathf.Rad2Deg - 90.0f;
        float currentAngle = (transform.rotation.eulerAngles.z + 90.0f) * Mathf.Deg2Rad;
        Quaternion newRotation = transform.rotation;
        if (direction.value != Vector2.zero) 
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
                currentSpeedMultiplier.value = speedMultiplier;
                currentSpeedo.value /= currentSpeedMultiplier.value;
                break;
            case MovementState.MOVING:
                currentSpeedo.value *= currentSpeedMultiplier.value * 5.0f;
                break;
            case MovementState.CHANGING_DIRECTION:
                if (currentAngle - angle > 60.0f) {
                    currentSpeedMultiplier.value = speedMultiplier;
                } else {
                    currentSpeedo.value *= currentSpeedMultiplier.value;
                } 
                break;
        }
            
        if (currentSpeedo.value > checkingMaxSpeedo) {
            currentSpeedo.value = checkingMaxSpeedo;
        }
        if (currentSpeedo.value < minSpeedo) {
            currentSpeedo.value = minSpeedo;
        }

        Vector3 currentDirection = new Vector3(Mathf.Cos(currentAngle) , Mathf.Sin(currentAngle), 0.0f);
        transform.position = initPosition + currentDirection * currentSpeedo.value * deltaTime;
    }

    public override void Simulate() {
        MoveSpaceship(transform.position, Time.fixedDeltaTime);
    }

    public override void GoToFrame(int frameNumber) {
        currentSpeedMultiplier.SetValueFromFrameNumber(frameNumber);
        currentSpeedo.SetValueFromFrameNumber(frameNumber);
        direction.SetValueFromFrameNumber(frameNumber);
        positionRB.SetValueFromFrameNumber(frameNumber);
        rotationRB.SetValueFromFrameNumber(frameNumber);

        transform.position = positionRB.value;
        transform.rotation = rotationRB.value;
    }

    public override void DeleteFrames(int fromFrame, int toFrame) {
        currentSpeedMultiplier.DeleteFrames(fromFrame, toFrame);
        currentSpeedo.DeleteFrames(fromFrame, toFrame);
        direction.DeleteFrames(fromFrame, toFrame);
        positionRB.DeleteFrames(fromFrame, toFrame);
        rotationRB.DeleteFrames(fromFrame, toFrame);
    }

    public override void SaveFrame() {
        currentSpeedMultiplier.SaveFrame();
        currentSpeedo.SaveFrame();
        direction.SaveFrame();
        positionRB.SetAndSaveValue(transform.position);
        rotationRB.SetAndSaveValue(transform.rotation);
    }
}
