using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(RollbackComponent))]
public class RollbackComponentInspector : Editor {
    RollbackComponent _rollbackComponent;
    
    void OnSceneGUI() {
        if (_rollbackComponent == null) {
            _rollbackComponent = (RollbackComponent)target;
            RefreshComponentList();
        }
    }

    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        GUILayout.Label("Components");
        
        if (GUILayout.Button("Refresh list")) {
            RefreshComponentList();
        }
        var copy = _rollbackComponent.rollbackedComponents.ToList();
        foreach(var component in copy) {
            _rollbackComponent.rollbackedComponents[component.Key] = GUILayout.Toggle(component.Value, component.Key);
        }
    }

    private void RefreshComponentList() {
        Component[] components = _rollbackComponent.gameObject.GetComponents<Component>();

        Dictionary<string, bool> copiedDico = _rollbackComponent.rollbackedComponents.ToDictionary(entry => entry.Key, entry => entry.Value);
        _rollbackComponent.rollbackedComponents.Clear();
        
        foreach(var component in components) {
            bool temp = false;
            copiedDico.TryGetValue(component.GetType().ToString(), out temp);
            Debug.Log(component.GetType().ToString() + " : " + temp);
            _rollbackComponent.rollbackedComponents.Add(component.GetType().ToString(), temp);
        }
    }
}
