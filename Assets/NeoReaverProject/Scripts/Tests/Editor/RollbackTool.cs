using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[Serializable]
public class RollbackTool : EditorWindow {
    public static RollbackInformation rollbackInformation = new RollbackInformation();
    public static RollbackInformation completeRollbackInformation = new RollbackInformation();

    bool openedObjectList = false;
    int currentWantedSize = 0;
    
    [MenuItem("RollbackTool/Information")]
    public static void ShowWindow() {

        UpdateInformationList();
        //rollbackObjectsList = new List<Object>(); 
        //instancesIdList = new List<int>();
        GetWindow(typeof(RollbackTool));
    }

    void OnGUI() {
        openedObjectList = EditorGUILayout.Foldout(openedObjectList, "Rollback object list");
        if (openedObjectList) {
            
            currentWantedSize += 1;
            
            completeRollbackInformation.Clear();
            
            rollbackInformation.Resize(currentWantedSize);

            for (int i = 0; i < currentWantedSize; i++) {
                //Display list of game objects to track rollback on 
                EditorGUILayout.BeginHorizontal();
                rollbackInformation.objectsToRollback[i] = EditorGUILayout.ObjectField("object " + i.ToString(), rollbackInformation.objectsToRollback[i], typeof(GameObject), true, GUILayout.ExpandWidth(true)) as GameObject;
                if (i != currentWantedSize) {
                    if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20))) {
                        Debug.Log("Try");
                        if (rollbackInformation.objectsToRollback[i].GetComponent<RollbackComponent>() != null) {
                            Debug.Log("Remove Component");
                            Debug.Log(rollbackInformation.objectsToRollback[i].GetComponent<RollbackComponent>());
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
                    RefreshCompleteList();
                    currentWantedSize = i;
                    break;
                }
            }
        }
    }

    static void RefreshCompleteList() {
        completeRollbackInformation.Clear();
        for (int i = 0; i < rollbackInformation.objectsToRollback.Count; i++) {
            if (rollbackInformation.objectsToRollback[i] != null) {
                AddChildrenToList(rollbackInformation.objectsToRollback[i]);
            }
        }
    }

    static void AddChildrenToList(GameObject gameObject) {
        completeRollbackInformation.Add(gameObject);
        int numChildren = gameObject.transform.childCount;
        for (int i = 0; i < numChildren; i++) {
            GameObject newObj = gameObject.transform.GetChild(i).gameObject;
            completeRollbackInformation.Add(newObj);
            
            AddChildrenToList(newObj);
        }
    }

    private static void UpdateInformationList() {
        rollbackInformation.Clear();
        var list = GameObject.FindObjectsOfType<RollbackComponent>();
        foreach (RollbackComponent rollbackComponent in list) {
            rollbackInformation.Add(rollbackComponent.gameObject);
        }
    }
}
