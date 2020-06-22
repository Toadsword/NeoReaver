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
        
        //Button to refresh the list
        if (GUILayout.Button("Refresh list")) {
            RefreshComponentList();
        }
        
        //Display all the elements in the dictionnaries
        for(int i = 0; i < _rollbackComponent.rollbackedComponentsName.Count; i++){
            _rollbackComponent.doRollbackComponents[i] = GUILayout.Toggle(_rollbackComponent.doRollbackComponents[i], _rollbackComponent.rollbackedComponentsName[i]);
        }
    }

    private void RefreshComponentList() {
        Component[] components = _rollbackComponent.gameObject.GetComponents<Component>();

        Dictionary<string, bool> copiedDico = new Dictionary<string, bool>();
        for (int i = 0; i < _rollbackComponent.rollbackedComponentsName.Count; i++) {
            copiedDico.Add(_rollbackComponent.rollbackedComponentsName[i], _rollbackComponent.doRollbackComponents[i]);
        }

        _rollbackComponent.rollbackedComponentsName.Clear();
        _rollbackComponent.doRollbackComponents.Clear();
        
        foreach(var component in components) {
            bool temp = false;
            copiedDico.TryGetValue(component.GetType().ToString(), out temp);
            _rollbackComponent.rollbackedComponentsName.Add(component.GetType().ToString());
            _rollbackComponent.doRollbackComponents.Add(temp);
        }
    }
}
