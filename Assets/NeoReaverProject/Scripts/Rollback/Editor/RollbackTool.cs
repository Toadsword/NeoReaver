using System;
using UnityEditor;
using UnityEngine;

[Serializable]
public class RollbackTool : EditorWindow {
    public static RollbackInformation rollbackInformation = new RollbackInformation();

    RollbackManager _rollbackManager;
    bool justCreatedManager = false;
    
    bool openedObjectList = false;
    int currentWantedSize = 0;
    
    [MenuItem("RollbackTool/Information")]
    public static void ShowWindow() {

        UpdateInformationList();
        //rollbackObjectsList = new List<Object>(); 
        //instancesIdList = new List<int>();
        GetWindow(typeof(RollbackTool));
    }
    
    RollbackTool() {
        EditorApplication.playModeStateChanged += LogPlayModeState;
    }

    void OnGUI() {
        DisplayRollbackEditionButtons();
        
        DisplayAllRollbackEntities();
    }

    private void LogPlayModeState(PlayModeStateChange playModeStateChange) {
        switch (playModeStateChange) {
            case PlayModeStateChange.EnteredPlayMode:
                _rollbackManager = GameObject.FindObjectOfType<RollbackManager>(); 
                if (_rollbackManager == null) {
                    _rollbackManager = Instantiate(Resources.Load("RollbackManagerPrefab") as GameObject, Vector3.zero, Quaternion.identity).GetComponent<RollbackManager>();
                    justCreatedManager = true;
                }
                break;
            case PlayModeStateChange.EnteredEditMode: {
                if (justCreatedManager) {
                    justCreatedManager = false;
                    Destroy(_rollbackManager.gameObject);
                }
                break;
            }
        }
    }

    private static void UpdateInformationList() {
        rollbackInformation.Clear();
        var list = GameObject.FindObjectsOfType<RollbackComponent>();
        foreach (RollbackComponent rollbackComponent in list) {
            rollbackInformation.Add(rollbackComponent.gameObject);
        }
    }

    private void DisplayRollbackEditionButtons() {
        EditorGUILayout.BeginHorizontal();
        
        if (GUILayout.Button("<", GUILayout.Width(20), GUILayout.Height(20))) {
            
        }

        if (UnityEditor.EditorApplication.isPlaying){
            if (GUILayout.Button("Stop", GUILayout.Width(100), GUILayout.Height(20))) {
                UnityEditor.EditorApplication.isPlaying = false;
            }
        } else {
            if (GUILayout.Button("Play", GUILayout.Width(100), GUILayout.Height(20))) {
                UnityEditor.EditorApplication.isPlaying = true;
            }
        }

        if (UnityEditor.EditorApplication.isPaused) {
            if (GUILayout.Button("Resume", GUILayout.Width(100), GUILayout.Height(20))) {
                UnityEditor.EditorApplication.isPaused = false;
            }
        } else {
            if (GUILayout.Button("Pause", GUILayout.Width(100), GUILayout.Height(20))) {
                UnityEditor.EditorApplication.isPaused = true;
            }
        }

        if (GUILayout.Button(">", GUILayout.Width(20), GUILayout.Height(20))) {
            
        }
        
        EditorGUILayout.EndHorizontal();
    }
    private void DisplayAllRollbackEntities() {
        openedObjectList = EditorGUILayout.Foldout(openedObjectList, "Rollback object list");
        if (openedObjectList) {

            currentWantedSize += 1;

            rollbackInformation.Resize(currentWantedSize);

            for (int i = 0; i < currentWantedSize; i++) {
                //Display list of game objects to track rollback on 
                EditorGUILayout.BeginHorizontal();
                rollbackInformation.objectsToRollback[i] = EditorGUILayout.ObjectField("object " + i.ToString(), rollbackInformation.objectsToRollback[i], typeof(GameObject), true, GUILayout.ExpandWidth(true)) as GameObject;
                if (i != currentWantedSize) {
                    if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20))) {
                        if (rollbackInformation.objectsToRollback[i].GetComponent<RollbackComponent>() != null) {
                            DestroyImmediate(rollbackInformation.objectsToRollback[i].GetComponent<RollbackComponent>());
                        }
                        rollbackInformation.RemoveAt(i);
                        currentWantedSize = rollbackInformation.GetCount();
                        break;
                    }
                }
                EditorGUILayout.EndHorizontal();
                
                if (rollbackInformation.objectsToRollback[i] != null) {
                    rollbackInformation.instancesIdToRollback[i] = rollbackInformation.objectsToRollback[i].GetInstanceID();
                    
                    //If the object doesn't have the component, add it
                    if (rollbackInformation.objectsToRollback[i].GetComponent<RollbackComponent>() == null) {
                        rollbackInformation.objectsToRollback[i].AddComponent<RollbackComponent>();
                    }
                } else {
                    currentWantedSize = i;
                    break;
                }
            }
        }
    }
}
