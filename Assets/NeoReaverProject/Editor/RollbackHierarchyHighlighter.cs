using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class RollbackHierarchyHighligher {

    List<int> objectsEntitiesToRollback = new List<int>();
    
    //==============================================================================
    //
    //                                    CONSTANTS
    //
    //==============================================================================
        
    public static readonly Color DEFAULT_COLOR_HIERARCHY_SELECTED = new Color(0.243f, 0.4901f, 0.9058f, 1f);

    //==============================================================================
    //
    //                                    CONSTRUCTORS
    //
    //==============================================================================

    static RollbackHierarchyHighligher()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyHighlight_OnGUI;
    }

    //==============================================================================
    //
    //                                    EVENTS
    //
    //==============================================================================
    private static void HierarchyHighlight_OnGUI(int inSelectionID, Rect inSelectionRect) {

        bool mustRollbackObj = RollbackTool.completeRollbackInformation.Contains(inSelectionID);

        GameObject GO_Label = EditorUtility.InstanceIDToObject(inSelectionID) as GameObject;

        if (GO_Label != null) {

            if (mustRollbackObj) {
                Rect backgroundOffset = new Rect(inSelectionRect.position, inSelectionRect.size);   
                
                bool ObjectIsSelected = Selection.instanceIDs.Contains(inSelectionID);
                if (ObjectIsSelected)
                    EditorGUI.DrawRect(backgroundOffset, Color.Lerp(GUI.skin.settings.selectionColor, Color.green, 0.5f));
                else
                    EditorGUI.DrawRect(backgroundOffset, Color.green);
                
                Rect Offset = new Rect(inSelectionRect.position + new Vector2(20f, 0f), inSelectionRect.size);
                EditorGUI.LabelField(Offset, GO_Label.name, new GUIStyle()
                {
                    normal = new GUIStyleState() { textColor = Color.black }
                });
            }

            EditorApplication.RepaintHierarchyWindow();
        }
    }
}
