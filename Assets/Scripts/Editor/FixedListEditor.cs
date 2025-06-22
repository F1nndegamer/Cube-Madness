#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(ExtraTileTrigger))]
public class ExtraTileTriggerEditor : UnityEditor.Editor
{
    private ReorderableList axisList;

    private void OnEnable()
    {
        SerializedProperty axisProp = serializedObject.FindProperty("movementAxisOrder");

        axisList = new ReorderableList(serializedObject, axisProp, true, true, false, false);

        axisList.drawHeaderCallback = (Rect rect) =>
        {
            EditorGUI.LabelField(rect, "Movement Axis Order (reorder only)");
        };

        axisList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
        {
            SerializedProperty element = axisList.serializedProperty.GetArrayElementAtIndex(index);
            rect.y += 2;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(rect, element, GUIContent.none);
            EditorGUI.EndDisabledGroup();
        };
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        axisList.DoLayoutList();
        DrawPropertiesExcluding(serializedObject, "movementAxisOrder");
        serializedObject.ApplyModifiedProperties();
    }
}
#endif