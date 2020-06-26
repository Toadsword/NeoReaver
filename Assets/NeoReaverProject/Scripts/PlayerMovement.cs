using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IRollbackComponent {

    [Serializable]
    enum MovementState {
        IDLE,
        MOVING,
        CHANGING_DIRECTION
    }
    
    [SerializeField] float maxSpeedo = 5.0f;
    [SerializeField] float minSpeedo = 0.2f;
    [SerializeField] float speedMultiplier;
    [SerializeField] float currentSpeedMultiplier;
    public Vector2 direction;


    Vector3 fixedLastUpdatePosition;
    [SerializeField] float midSpeedo;
    [SerializeField] float currentSpeedo;
    [SerializeField] MovementState currentMovementState;

    void Start() {
        currentSpeedo = 5.0f;
        midSpeedo = (maxSpeedo + minSpeedo) / 2.0f;
        currentSpeedMultiplier = speedMultiplier;
    }

    void Update() {
        MoveSpaceship(transform.position, Time.deltaTime);
    }

    private void MoveSpaceship(Vector3 initPosition, float deltaTime) {

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90.0f;
        float currentAngle = (transform.rotation.eulerAngles.z + 90.0f) * Mathf.Deg2Rad;
        Quaternion newRotation = transform.rotation;
        if (direction != Vector2.zero) 
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
                currentSpeedMultiplier = speedMultiplier;
                currentSpeedo /= currentSpeedMultiplier;
                break;
            case MovementState.MOVING:
                currentSpeedo *= currentSpeedMultiplier * 5.0f;
                break;
            case MovementState.CHANGING_DIRECTION:
                if (currentAngle - angle > 60.0f) {
                    currentSpeedMultiplier = speedMultiplier;
                } else {
                    currentSpeedo *= currentSpeedMultiplier;
                } 
                break;
        }
            
        if (currentSpeedo > checkingMaxSpeedo) {
            currentSpeedo = checkingMaxSpeedo;
        }
        if (currentSpeedo < minSpeedo) {
            currentSpeedo = minSpeedo;
        }

        Vector3 currentDirection = new Vector3(Mathf.Cos(currentAngle) , Mathf.Sin(currentAngle), 0.0f);
        transform.position = initPosition + currentDirection * currentSpeedo * deltaTime;
    }

    public void Simulate() {
        Debug.Log("Moveing Spaceship from PlayerMovement");
        MoveSpaceship(transform.position, Time.fixedDeltaTime);
    }
}
