using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] float maxSpeedo = 5.0f;
    [SerializeField] float minSpeedo = 0.0f;
    [SerializeField] float speedMultiplier;
    public Vector2 direction;

    float midSpeedo;
    
    Vector3 fixedLastUpdatePosition;
    float speedo;

    void Start() {
        speedo = 5.0f;
        midSpeedo = (maxSpeedo + minSpeedo) / 2.0f;
    }

    void Update() {
        MoveSpaceship(transform.position, Time.deltaTime);
    }

    private void MoveSpaceship(Vector3 initPosition, float deltaTime) {

        if (direction != Vector2.zero) 
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90.0f;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.AngleAxis(angle, Vector3.forward), Time.deltaTime * 450.0f);

            
            
            if (speedo > maxSpeedo) {
                speedo = maxSpeedo;
            }
            if (speedo < minSpeedo) {
                speedo = minSpeedo;
            }
        }

        float currentAngle = (transform.rotation.eulerAngles.z + 90.0f) * Mathf.Deg2Rad;
        Vector3 currentDirection = new Vector3(Mathf.Cos(currentAngle) , Mathf.Sin(currentAngle), 0.0f);
        transform.position = initPosition + currentDirection * speedo * deltaTime;
    }
}
