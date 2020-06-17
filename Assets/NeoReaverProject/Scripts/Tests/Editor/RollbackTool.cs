using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RollbackTool : EditorWindow {
    public static List<GameObject> completeRollbackObjectsList = new List<GameObject>();
    public static List<int> completeInstancesIdList = new List<int>();
    
    public static List<GameObject> rollbackObjectsList = new List<GameObject>();
    public static List<int> instancesIdList = new List<int>();
    
    bool openedObjectList = false;
    int currentWantedSize = 0;
    
    [MenuItem("RollbackTool/Information")]
    public static void ShowWindow() {
        //rollbackObjectsList = new List<Object>(); 
        //instancesIdList = new List<int>();
        GetWindow(typeof(RollbackTool));
    }

    void OnGUI() {
        GUILayout.Label("Spawn New Object", EditorStyles.boldLabel);
        openedObjectList = EditorGUILayout.Foldout(openedObjectList, "Rollback object list");
        if (openedObjectList) {
            
            currentWantedSize += 1;
            
            completeRollbackObjectsList = new List<GameObject>();
            completeInstancesIdList = new List<int>();
            
            while (currentWantedSize < rollbackObjectsList.Count) {
                rollbackObjectsList.RemoveAt(rollbackObjectsList.Count - 1);
                instancesIdList.RemoveAt(instancesIdList.Count - 1);
            }

            while (currentWantedSize > rollbackObjectsList.Count) {
                rollbackObjectsList.Add(null);
                instancesIdList.Add(0);
            }

            for (int i = 0; i < currentWantedSize; i++) {
                //Debug.Log("occurence");
                //Display list of game objects to track rollback on 
                EditorGUILayout.BeginHorizontal();
                rollbackObjectsList[i] = EditorGUILayout.ObjectField("object " + i.ToString(), rollbackObjectsList[i], typeof(GameObject), true, GUILayout.ExpandWidth(true)) as GameObject;
                if (i != currentWantedSize) {
                    if (GUILayout.Button("X", GUILayout.Width(20), GUILayout.Height(20))) {
                        rollbackObjectsList.RemoveAt(i);
                        instancesIdList.RemoveAt(i);
                        currentWantedSize = rollbackObjectsList.Count;
                        break;
                    }
                }
                EditorGUILayout.EndHorizontal();
                
                if (rollbackObjectsList[i] != null) {
                    instancesIdList[i] = rollbackObjectsList[i].GetInstanceID();
                    EditorGUILayout.TextField(rollbackObjectsList[i].transform.childCount.ToString());
                    AddChildrenToList(rollbackObjectsList[i]);
                } else {
                    currentWantedSize = i;
                    break;
                }
            }
        }
        if (GUILayout.Button("Save")) {
            
        }
    }

    void AddChildrenToList(GameObject gameObject) {
        completeInstancesIdList.Add(gameObject.GetInstanceID());
        completeRollbackObjectsList.Add(gameObject);
        int numChildren = gameObject.transform.childCount;
        for (int i = 0; i < numChildren; i++) {
            completeRollbackObjectsList.Add(gameObject.transform.GetChild(i).gameObject);
            completeInstancesIdList.Add(gameObject.transform.GetChild(i).GetInstanceID());
            
            AddChildrenToList(gameObject.transform.GetChild(i).gameObject);
        }
    }
    
}
