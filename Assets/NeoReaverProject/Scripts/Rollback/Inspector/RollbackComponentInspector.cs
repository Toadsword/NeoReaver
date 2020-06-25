using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
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
        //base.OnInspectorGUI();

        if (_rollbackComponent == null) {
            _rollbackComponent = (RollbackComponent)target;
            RefreshComponentList();
        }
        
        GUILayout.Label("Components");
        
        //Button to refresh the list
        if (GUILayout.Button("Refresh list")) {
            RefreshComponentList();
        }

        CheckListsSize();
        
        
        //Header of list
        GUILayout.BeginHorizontal();
        
        GUILayout.Label("Component name");
        GUILayout.FlexibleSpace();
        GUILayout.Label("Track info when rollback");
        GUILayout.Label("Disable script when rollback");
        GUILayout.EndHorizontal();

        bool valueChanged = false;
        //Display all the elements in the dictionnaries
        for(int i = 0; i < _rollbackComponent.rollbackedComponentsName.Count; i++) {
            GUILayout.BeginHorizontal();
            
            bool tempRbComp = _rollbackComponent.doRollbackComponents[i];
            
            GUILayout.Label(_rollbackComponent.rollbackedComponentsName[i]);
            GUILayout.FlexibleSpace();
            _rollbackComponent.doRollbackComponents[i] = GUILayout.Toggle(_rollbackComponent.doRollbackComponents[i], GUIContent.none);
            GUILayout.EndHorizontal();

            if (tempRbComp != _rollbackComponent.doRollbackComponents[i]) {
                valueChanged = true;
            }
        }

        if (valueChanged) {
            if (GUI.changed)
            {
                EditorUtility.SetDirty(_rollbackComponent);
                EditorSceneManager.MarkSceneDirty(_rollbackComponent.gameObject.scene);
            }
        }
    }

    private void RefreshComponentList() {
        Component[] components = _rollbackComponent.gameObject.GetComponents<Component>();

        Dictionary<string, bool> copiedCompDico = new Dictionary<string, bool>();
        for (int i = 0; i < _rollbackComponent.rollbackedComponentsName.Count; i++) {
            copiedCompDico.Add(_rollbackComponent.rollbackedComponentsName[i], _rollbackComponent.doRollbackComponents.Count > i && _rollbackComponent.doRollbackComponents[i]);
        }

        _rollbackComponent.rollbackedComponentsName.Clear();
        _rollbackComponent.doRollbackComponents.Clear();

        foreach(var component in components) {
            
            _rollbackComponent.rollbackedComponentsName.Add(component.GetType().ToString());
            
            //DoRollBackComponent
            bool temp = false;
            copiedCompDico.TryGetValue(component.GetType().ToString(), out temp);
            _rollbackComponent.doRollbackComponents.Add(temp);
        }
    }

    private void CheckListsSize() {
        int wantedSize = _rollbackComponent.rollbackedComponentsName.Count;
        if (_rollbackComponent.doRollbackComponents.Count != wantedSize) {
            RefreshComponentList();
        }
    }
}
