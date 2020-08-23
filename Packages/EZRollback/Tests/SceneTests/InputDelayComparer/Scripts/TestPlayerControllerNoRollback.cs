using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerControllerNoRollback : MonoBehaviour
{
    [SerializeField] private float _horizontal = 0.0f;
    [SerializeField] private float _vertical = 0.0f;

    [SerializeField] Color _baseColor = Color.white;
    SpriteRenderer _spriteRenderer;

    void Start() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {
        _horizontal = Input.GetAxisRaw("Horizontal");
        _vertical = Input.GetAxisRaw("Vertical");
        
        _spriteRenderer.color = _baseColor; 
        if (Input.GetKey(KeyCode.D)) {
            _spriteRenderer.color = Color.cyan; 
        }
        if (Input.GetKeyDown(KeyCode.D)) {
            _spriteRenderer.color = Color.blue; 
        }
        if (Input.GetKeyUp(KeyCode.D)) {
            _spriteRenderer.color = Color.green; 
        }
    }

    void FixedUpdate() {
        float angle = Mathf.Atan2(_vertical, _horizontal) * Mathf.Rad2Deg - 90.0f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
