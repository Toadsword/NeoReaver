using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] float maxSpeedo;
    public Vector2 direction;
    
    Vector3 fixedLastUpdatePosition;
    float speedo;

    void Start() {
        
        speedo = 5.0f;
    }

    void Update() {
        MoveSpaceship(transform.position, Time.deltaTime);
    }

    private void MoveSpaceship(Vector3 initPosition, float deltaTime) {

        if (direction != Vector2.zero) 
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90.0f;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * 300.0f);

            if (speedo > maxSpeedo) {
                speedo = maxSpeedo;
            }
            if (speedo < 0.0f) {
                speedo = 0.0f;
            }
        }

        float currentAngle = (transform.rotation.eulerAngles.z + 90.0f) * Mathf.Deg2Rad;
        Vector3 currentDirection = new Vector3(Mathf.Cos(currentAngle) , Mathf.Sin(currentAngle), 0.0f);
        Debug.Log(currentDirection);
        transform.position = initPosition + currentDirection * speedo * deltaTime;
    }
}
