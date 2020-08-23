using System;
using System.Collections;
using System.Collections.Generic;
using Packages.EZRollback.Runtime.Scripts;
using UnityEditor;
using UnityEngine;

public class TestPlayerController : RollbackBehaviour
{
    [SerializeField] private float _horizontal = 0.0f;
    [SerializeField] private float _vertical = 0.0f;

    [SerializeField] Color _baseColor = Color.white;
    SpriteRenderer _spriteRenderer;

    RollbackElement<Color> _colors = new RollbackElement<Color>();
    
    void Start() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        RollbackManager.rbInputManager.AddPlayer();
    }
    // Update is called once per frame
    void Update() {
        Simulate();
    }
    
    public override void Simulate() {
        _horizontal = RollbackManager.rbInputManager.GetAxis(RollbackInputManager.AxisEnum.HORIZONTAL, 0);
        _vertical = RollbackManager.rbInputManager.GetAxis(RollbackInputManager.AxisEnum.VERTICAL, 0);
        
        _spriteRenderer.color = _baseColor;
        if (RollbackManager.rbInputManager.GetInput(1, 0)) {
            Debug.Log("Input : Cyan");
            _spriteRenderer.color = Color.cyan; 
        }
        if (RollbackManager.rbInputManager.GetInputDown(1, 0)) {
            Debug.Log("InputDown : Blue");
            _spriteRenderer.color = Color.blue; 
        }
        if (RollbackManager.rbInputManager.GetInputUp(1, 0)) {
            Debug.Log("InputUp : Green");
            _spriteRenderer.color = Color.green; 
        }
        
        float angle = Mathf.Atan2(_vertical, _horizontal) * Mathf.Rad2Deg - 90.0f;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        _colors.value = _spriteRenderer.color;
    }

    public override void SetValueFromFrameNumber(int frameNumber) {
        _colors.SetValueFromFrameNumber(frameNumber);
        _spriteRenderer.color = _colors.value;
    }

    public override void DeleteFrames(int numFramesToDelete, RollbackManager.DeleteFrameMode deleteMode) {
        _colors.DeleteFrames(numFramesToDelete, deleteMode);
    }

    public override void SaveFrame() {
        _colors.SaveFrame();
    }

}
