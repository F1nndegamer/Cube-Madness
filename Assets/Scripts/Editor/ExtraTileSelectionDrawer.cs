#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ExtraTileSelection))]
public class ExtraTileSelectionDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        int tilePrefab = property.FindPropertyRelative("tilePrefab").intValue;

        int lines = 2;
        if (tilePrefab == 1 || tilePrefab == 2) lines++;
        if (tilePrefab == 1) lines++;

        return EditorGUIUtility.singleLineHeight * lines + EditorGUIUtility.standardVerticalSpacing * (lines - 1);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int tilePrefab = property.FindPropertyRelative("tilePrefab").intValue;

        SerializedProperty tilePrefabProp = property.FindPropertyRelative("tilePrefab");
        SerializedProperty posProp = property.FindPropertyRelative("Pos");
        SerializedProperty blockPosProp = property.FindPropertyRelative("BlockPos");
        SerializedProperty movePositionProp = property.FindPropertyRelative("MovePosition");

        float lineHeight = EditorGUIUtility.singleLineHeight;
        float spacing = EditorGUIUtility.standardVerticalSpacing;

        Rect lineRect = new Rect(position.x, position.y, position.width, lineHeight);

        EditorGUI.PropertyField(lineRect, tilePrefabProp);

        lineRect.y += lineHeight + spacing;
        EditorGUI.PropertyField(lineRect, posProp);

        if (tilePrefab == 1 || tilePrefab == 2)
        {
            lineRect.y += lineHeight + spacing;
            EditorGUI.PropertyField(lineRect, blockPosProp, new GUIContent("BlockPos"));
        }

        if (tilePrefab == 1)
        {
            lineRect.y += lineHeight + spacing;
            EditorGUI.PropertyField(lineRect, movePositionProp, new GUIContent("MovePosition"));
        }
    }
}
#endif
