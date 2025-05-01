#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Transform))]
public class TransformSnapEditor : Editor
{
    private Vector3 previousPosition;

    private void OnEnable()
    {
        previousPosition = ((Transform)target).position;
    }

    private void OnSceneGUI()
    {
        Transform t = (Transform)target;

        // Only if the position changed (dragging)
        if (t.position != previousPosition)
        {
            if (Event.current.type == EventType.MouseUp)
            {
                Undo.RecordObject(t, "Snap Position");

                Vector3 snapped = new Vector3(
                    Mathf.Round(t.position.x),
                    Mathf.Round(t.position.y),
                    Mathf.Round(t.position.z)
                );

                t.position = snapped;
                EditorUtility.SetDirty(t);
            }

            previousPosition = t.position;
        }
    }
}
#endif
