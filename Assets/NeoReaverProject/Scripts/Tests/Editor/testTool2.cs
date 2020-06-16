using System;
using UnityEditor;
using UnityEngine;
using WebSocketSharp;

public class testTool2 : EditorWindow {
    string objectBaseName = "";
    int objectId = 1;
    GameObject objectToSpawn;

    [MenuItem("Tools/BasicObjectSpawner")]
    public static void ShowWindow() {
        GetWindow(typeof(testTool2));
    }

    void OnGUI() {
        GUILayout.Label("Spawn New Object", EditorStyles.boldLabel);

        objectBaseName = EditorGUILayout.TextField("Base name", objectBaseName);
        objectId = EditorGUILayout.IntField("Object ID", objectId);
        objectToSpawn = EditorGUILayout.ObjectField("Prefab to spawn", objectToSpawn, typeof(GameObject), false) as GameObject;

        if (GUILayout.Button("Spawn object")) {
            SpawnObject();
        }
    }

    void SpawnObject() {
        if (objectToSpawn == null) {
            Debug.LogError("test Tool error : Please assign an object to spawn");
            return;
        }

        if (objectBaseName.IsNullOrEmpty()) {
            Debug.LogError("test Tool error : Please enter a name for the object");
            return;
        }

        GameObject newGameObject = Instantiate(objectToSpawn, Vector3.zero, Quaternion.identity);
        newGameObject.name = objectBaseName + objectId.ToString();

        objectId++;
    }
}
