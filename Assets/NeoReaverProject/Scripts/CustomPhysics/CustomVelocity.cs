using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomVelocity : MonoBehaviour {
    [SerializeField] Vector3 speed = Vector3.zero;

    void FixedUpdate() {
        transform.position += speed * Time.fixedDeltaTime;
    }
}
