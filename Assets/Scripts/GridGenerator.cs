using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[System.Serializable]
public class ExtraTileSelection
{
    public GameObject tilePrefab = null;
    public Vector3 Pos = Vector3.zero;
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

        // Maak een HashSet voor snellere opzoekingen van extra tiles
        HashSet<Vector2> extraTilePositions = new HashSet<Vector2>();
        foreach (var extra in extratiles)
        {
            extraTilePositions.Add(new Vector2(extra.Pos.x, extra.Pos.z));
        }

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                // Skip als de positie in extraTilePositions zit
                if (extraTilePositions.Contains(new Vector2(x, z))) continue;

                Vector3 tilePosition = new Vector3(x * tileSize, 0, z * tileSize);
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, transform);
                tile.name = $"Tile ({x}, {z})";
            }
        }

        // Plaats de extra tiles op de correcte posities
        foreach (ExtraTileSelection extraTile in extratiles)
        {
            if (extraTile.tilePrefab != null)
            {
                Vector3 extraTilePosition = new Vector3(extraTile.Pos.x * tileSize, extraTile.Pos.y, extraTile.Pos.z * tileSize);
                GameObject extra = Instantiate(extraTile.tilePrefab, extraTilePosition, Quaternion.identity, transform);
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
