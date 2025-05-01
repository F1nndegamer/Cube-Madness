#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;
using UnityEditor.Build;

[CustomEditor(typeof(Transform))]
public class TransformSnapEditor : Editor
{
    private void OnSceneGUI()
    {
        Transform t = (Transform)target;

        if (t.parent != null)
        {
            PlayerRollingMovement playerscript = t.gameObject.GetComponent<PlayerRollingMovement>();
            if (t.parent.name == "Environment" || playerscript != null)
            {
                if (Event.current.type == EventType.MouseUp)
                {
                    Vector3 snappedPosition = new Vector3(
                        Mathf.Round(t.position.x),
                        Mathf.Round(t.position.y),
                        Mathf.Round(t.position.z)
                    );

                    t.position = snappedPosition;
                    EditorUtility.SetDirty(t);
                    string pattern = @"\(\s*-?\d+\s*,\s*-?\d+\s*\)";
                    string replacement = $"({(int)snappedPosition.x}, {(int)snappedPosition.z})";

                    if (Regex.IsMatch(t.name, pattern))
                    {
                        t.name = Regex.Replace(t.name, pattern, replacement);
                    }

                }
            }
        }
    }
}
#endif
