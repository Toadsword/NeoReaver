using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using EZRollback.Core.Component;
using UnityEngine;

public class Movement : MonoBehaviour {
    [SerializeField] Vector2 _movementSpeed = Vector2.zero;

    void Update() {
        transform.position += (Vector3)_movementSpeed * Time.deltaTime;
    }

    public void SetSpeed(Vector2 newSpeed) {
        _movementSpeed = newSpeed;
    }
}
