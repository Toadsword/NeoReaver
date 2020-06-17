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

        bool mustRollbackObj = RollbackTool.instancesIdList.Contains(inSelectionID);

        GameObject GO_Label = EditorUtility.InstanceIDToObject(inSelectionID) as GameObject;

        if (GO_Label != null) {

            if (mustRollbackObj) {
                Rect backgroundOffset = new Rect(inSelectionRect.position, inSelectionRect.size);   
                EditorGUI.DrawRect(backgroundOffset, Color.Lerp(GUI.skin.settings.selectionColor, Color.green, 0.3f));
            }

            EditorApplication.RepaintHierarchyWindow();
        }
    }
}
