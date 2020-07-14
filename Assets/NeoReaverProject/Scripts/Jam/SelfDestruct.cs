using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestruct : MonoBehaviour {
    [SerializeField] int _timeBeforeSelfDestruct = 100;

    Timer _timer;
    
    void Start() {
        _timer = new Timer(_timeBeforeSelfDestruct);
        SetGameobjectActive();
    }

    public void SetGameobjectActive() {
        gameObject.SetActive(true);
        _timer.Reset();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (_timer.ShouldExecute) {
            gameObject.SetActive(false);
        }
    }
}
