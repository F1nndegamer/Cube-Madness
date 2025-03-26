using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[System.Serializable]
public class ExtraTileSelection
{
    public GameObject tilePrefab = null;
    public Vector3 Pos = new Vector3(0, 0, 0);
}

public class GridGenerator : MonoBehaviour
{
    public GameObject tilePrefab;
    public int gridWidth = 5;
    public int gridHeight = 5;
    public float tileSize = 1f;
    public List<ExtraTileSelection> extratiles = new List<ExtraTileSelection>();
    public void GenerateGrid()
    {
        if (tilePrefab == null)
        {
            Debug.LogWarning("Assign a Tile Prefab");
            return;
        }

        ClearGrid();

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                foreach (ExtraTileSelection extraTile in extratiles)
                {
                    if (extraTile.Pos.x != x || extraTile.Pos.z != z)// if the tile is in extratiles skip it
                    {
                        Vector3 tilePosition = new Vector3(x * tileSize, 0, z * tileSize);
                        GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, transform);
                        tile.name = $"Tile ({x}, {z})";
                    }
                }
            }
        }
        foreach (ExtraTileSelection extraTile in extratiles)
        {
            if (extraTile.tilePrefab != null)
            {
                GameObject extra = Instantiate(extraTile.tilePrefab, extraTile.Pos, Quaternion.identity, transform);
                extra.name = $"Extra Tile ({extraTile.Pos.x}, {extraTile.Pos.z})";
            }
        }

    }

    public void ClearGrid()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GridGenerator))]
public class GridGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridGenerator gridGenerator = (GridGenerator)target;

        if (GUILayout.Button("Generate Grid"))
        {
            gridGenerator.GenerateGrid();
            EditorUtility.SetDirty(gridGenerator);
        }

        if (GUILayout.Button("Clear Grid"))
        {
            gridGenerator.ClearGrid();
            EditorUtility.SetDirty(gridGenerator);
        }
    }
}
#endif
