#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

[CustomEditor(typeof(Transform))]
public class TransformSnapEditor : UnityEditor.Editor
{
    private void OnSceneGUI()
    {
        Transform t = (Transform)target;
        if (t.parent != null)
        {
            PlayerRollingMovement playerscript = t.gameObject.GetComponent<PlayerRollingMovement>();
            if ((t.parent != null && t.parent.name == "Environment") ||
            (t.parent != null && t.parent.parent != null && t.parent.parent.name == "Environment") ||
          playerscript != null)

            {
                if (Event.current.type == EventType.MouseUp)
                {
                    Vector3 position = t.position;
                    if (t.name.StartsWith("WallZ"))
                    {
                        SnapToInterval(ref position, true);
                    }
                    else if (t.name.StartsWith("WallX"))
                    {
                        SnapToInterval(ref position, false);
                    }
                    else
                    {
                        SnapToInteger(ref position);
                    }
                    t.position = position;
                    EditorUtility.SetDirty(t);
                    UpdateNameWithPosition(position);
                }
            }
        }
    }

    private void SnapToInterval(ref Vector3 position, bool isWallZ)
    {
        if (isWallZ)
        {
            position.x = Mathf.Round(position.x * 2) / 2f;
            if (position.x == Mathf.Floor(position.x)) position.x += 0.5f;
            position.z = Mathf.Round(position.z);
        }
        else
        {
            position.z = Mathf.Round(position.z * 2) / 2f;
            if (position.z == Mathf.Floor(position.z)) position.z += 0.5f;
            position.x = Mathf.Round(position.x);
        }
        position.y = Mathf.Round(position.y);
        position.y -= 0.2f;
    }

    private void SnapToInteger(ref Vector3 position)
    {
        position.x = Mathf.Round(position.x);
        position.y = Mathf.Round(position.y);
        position.z = Mathf.Round(position.z);
    }

    private void UpdateNameWithPosition(Vector3 position)
    {
        string pattern = @"\(\s*-?\d+\s*,\s*-?\d+\s*,\s*-?\d+\s*\)";
        string replacement = $"({(int)position.x}, {(int)position.y}, {(int)position.z})";
        if (Regex.IsMatch(target.name, pattern))
        {
            target.name = Regex.Replace(target.name, pattern, replacement);
        }
    }
}
#endif
