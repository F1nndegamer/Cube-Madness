using UnityEngine;
using UnityEditor;
using UnityEditorInternal;


public class AxisStringOrderEditor : UnityEditor.Editor
{
    private ReorderableList reorderableList;

    private void OnEnable()
    {
        SerializedProperty listProp = serializedObject.FindProperty("axes");

        reorderableList = new ReorderableList(serializedObject, listProp, true, true, false, false);

        reorderableList.drawHeaderCallback = rect =>
        {
            EditorGUI.LabelField(rect, "Axes (Reorderable)");
        };

        reorderableList.drawElementCallback = (rect, index, isActive, isFocused) =>
        {
            SerializedProperty element = listProp.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.PropertyField(rect, element, GUIContent.none);
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        reorderableList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}