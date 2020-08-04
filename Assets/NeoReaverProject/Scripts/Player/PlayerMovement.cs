using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using NeoReaverProject.Scripts;
using UnityEngine;
using Packages.EZRollback.Runtime.Scripts;
using UnityEditor;


[Serializable]
public enum MovementState {
    IDLE,
    MOVING,
    CHANGING_DIRECTION
}

[Serializable]
public struct SpeedValues {
    public float speedo;
    public float speedMultiplier;
    public Vector2 direction;
    public MovementState movementState;
}

[Serializable]
public class RollbackElementSpeedValues : RollbackElement<SpeedValues> { }

public class PlayerMovement : RollbackBehaviour {

    [SerializeField] public bool movable = false;
    [SerializeField] Vector2 _direction = new Vector2();

    [SerializeField] float maxSpeedo = 5.0f;
    [SerializeField] float minSpeedo = 0.2f;
    [SerializeField] float speedMultiplier;

    Vector3 fixedLastUpdatePosition;
    [SerializeField] float midSpeedo;
    [SerializeField] public Transform spriteTransform;
    
    
    [SerializeField] public RollbackElementSpeedValues rbElements = new RollbackElementSpeedValues();

    PlayerController _playerController;

    public void SetDirection(Vector2 newDirection) {
        _direction = newDirection.normalized;
    }
    
    void Start() {
        _playerController = GetComponent<PlayerController>();
        rbElements.value.speedo = 5.0f;
        rbElements.value.speedMultiplier = speedMultiplier;
        
        midSpeedo = (maxSpeedo + minSpeedo) / 2.0f;
    }

    private void MoveSpaceship() {
        if (!movable) {
            return;
        }
        rbElements.value.direction = new Vector2(_direction.x, _direction.y);
        
        float newAngle = Mathf.Atan2(rbElements.value.direction.y, rbElements.value.direction.x) * Mathf.Rad2Deg - 90.0f;
        
        Quaternion newRotation = spriteTransform.rotation;
        float currentAngle = (newRotation.eulerAngles.z + 90.0f) * Mathf.Deg2Rad;
        if (rbElements.value.direction != Vector2.zero) 
        {
            newRotation = Quaternion.RotateTowards(newRotation, Quaternion.AngleAxis(newAngle, Vector3.forward), Time.fixedDeltaTime * 450.0f);
            // Calculating new state
            if (spriteTransform.rotation != newRotation) {
                rbElements.value.movementState = MovementState.CHANGING_DIRECTION;
            } else {
                rbElements.value.movementState = MovementState.MOVING;
            }
            spriteTransform.rotation = newRotation;
        } else {
            rbElements.value.movementState = MovementState.IDLE;
        }

        float checkingMaxSpeedo = maxSpeedo;
        switch (rbElements.value.movementState) {
            case MovementState.IDLE:
                rbElements.value.speedMultiplier = speedMultiplier;
                rbElements.value.speedo /= rbElements.value.speedMultiplier;
                break;
            case MovementState.MOVING:
                rbElements.value.speedo *= rbElements.value.speedMultiplier * 5.0f;
                break;
            case MovementState.CHANGING_DIRECTION:
                if (currentAngle - newAngle > 60.0f) {
                    rbElements.value.speedMultiplier = speedMultiplier;
                } else {
                    rbElements.value.speedo *= rbElements.value.speedMultiplier;
                } 
                break;
        }
            
        if (rbElements.value.speedo > checkingMaxSpeedo) {
            rbElements.value.speedo = checkingMaxSpeedo;
        }
        if (rbElements.value.speedo < minSpeedo) {
            rbElements.value.speedo = minSpeedo;
        }

        Vector3 currentDirection = new Vector3(Mathf.Cos(currentAngle) , Mathf.Sin(currentAngle), 0.0f);
        transform.position = transform.position + currentDirection * rbElements.value.speedo * Time.fixedDeltaTime;
    }

    public override void Simulate() {
        MoveSpaceship();
    }

    public override void SetValueFromFrameNumber(int frameNumber) {
        rbElements.SetValueFromFrameNumber(frameNumber);
    }

    public override void DeleteFrames(int numFramesToDelete,RollbackManager.DeleteFrameMode deleteMode) {
        rbElements.DeleteFrames(numFramesToDelete, deleteMode);
    }

    public override void SaveFrame() {
        rbElements.SaveFrame();
    }
}
