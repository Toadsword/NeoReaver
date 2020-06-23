using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class RollbackHierarchyHighligher {
   
    static RollbackHierarchyHighligher() {
        //RefreshCompleteList();
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyHighlight_OnGUI;
    }

    private static void HierarchyHighlight_OnGUI(int inSelectionID, Rect inSelectionRect) {

        GameObject GO_Label = EditorUtility.InstanceIDToObject(inSelectionID) as GameObject;
        
        if (GO_Label != null) {
            if (GO_Label.GetComponent<RollbackComponent>() || GO_Label.GetComponentInParent<RollbackComponent>()) {

                Rect backgroundOffset = new Rect(inSelectionRect.position, inSelectionRect.size);

                if (Selection.instanceIDs.Contains(inSelectionID)) {
                    EditorGUI.DrawRect(backgroundOffset, Color.Lerp(GUI.skin.settings.selectionColor, Color.green, 0.5f));
                } else {
                    EditorGUI.DrawRect(backgroundOffset, Color.green);
                }

                Rect Offset = new Rect(inSelectionRect.position + new Vector2(20f, 0f), inSelectionRect.size);
                EditorGUI.LabelField(Offset, GO_Label.name, new GUIStyle() {
                    normal = new GUIStyleState() {textColor = Color.black}
                });

                EditorApplication.RepaintHierarchyWindow();
            }
        }
    }
}
