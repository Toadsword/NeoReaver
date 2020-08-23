using UnityEditor;
using UnityEngine;

namespace Packages.EZRollback.Editor.Utils {
public static class GUIUtils
{
    /*
     * \brief Display a line in the editor. Intended to work as a separator
     */
    public static void GuiLine(int height = 1)
    {
        Rect rect = EditorGUILayout.GetControlRect(false, height );
        rect.height = height;
        EditorGUI.DrawRect(rect, new Color ( 0.5f,0.5f,0.5f, 1 ) );
    }
}
}
