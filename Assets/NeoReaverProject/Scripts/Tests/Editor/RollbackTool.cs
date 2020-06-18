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
        LoadInformations();
        //rollbackObjectsList = new List<Object>(); 
        //instancesIdList = new List<int>();
        GetWindow(typeof(RollbackTool));
    }

    void OnGUI() {
        GUILayout.Label("Spawn New Object", EditorStyles.boldLabel);
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
                        rollbackInformation.RemoveAt(i);
                        currentWantedSize = rollbackInformation.GetCount();
                        break;
                    }
                }
                EditorGUILayout.EndHorizontal();
                
                if (rollbackInformation.objectsToRollback[i] != null) {
                    rollbackInformation.instancesIdToRollback[i] = rollbackInformation.objectsToRollback[i].GetInstanceID();
                } else {
                    RefreshCompleteList();
                    currentWantedSize = i;
                    break;
                }
            }
        }
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save")) {
            SaveInformations();
        }
        if (GUILayout.Button("Load from last save")) {
            LoadInformations();
            currentWantedSize = rollbackInformation.GetCount();
            RefreshCompleteList();
        }
        EditorGUILayout.EndHorizontal();
    }

    void SaveInformations() {
        string saveJson = JsonUtility.ToJson(rollbackInformation);
        File.WriteAllText(Application.dataPath + "/save.txt", saveJson);
    }

    static void LoadInformations() {
        string json = File.ReadAllText(Application.dataPath + "/save.txt");
        rollbackInformation = JsonUtility.FromJson<RollbackInformation>(json);
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
        completeRollbackInformation.Add(ref gameObject);
        int numChildren = gameObject.transform.childCount;
        for (int i = 0; i < numChildren; i++) {
            GameObject newObj = gameObject.transform.GetChild(i).gameObject;
            completeRollbackInformation.Add(ref newObj);
            
            AddChildrenToList(newObj);
        }
    }
}
