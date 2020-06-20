using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] float speedo;
    public Vector2 direction;

    Vector3 fixedLastUpdatePosition;
    
    void Update() {
        MoveSpaceship(transform.position, Time.deltaTime);
    }
    
    void FixedUpdate() {
        //MoveSpaceship(fixedLastUpdatePosition, Time.fixedDeltaTime);
        //fixedLastUpdatePosition = transform.position;
    }

    private void MoveSpaceship(Vector3 initPosition, float deltaTime) {
        transform.position = initPosition + (Vector3)direction * speedo * deltaTime;
    }
}
