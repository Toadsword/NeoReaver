using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CustomVelocity))]
public class CustomVelocityInspector : Editor {

    CustomVelocity _customVelocity;

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();
        
        
    }
}
