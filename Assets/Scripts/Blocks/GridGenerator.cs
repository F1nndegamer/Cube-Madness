using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

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

        HashSet<Vector2> extraTilePositions = new HashSet<Vector2>();
        foreach (var extra in extratiles)
        {
            extraTilePositions.Add(new Vector2(extra.Pos.x, extra.Pos.z));
        }

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                if (extraTilePositions.Contains(new Vector2(x, z))) continue;

                Vector3 tilePosition = new Vector3(x * tileSize, 0, z * tileSize);
#if UNITY_EDITOR
                GameObject tile = (GameObject)PrefabUtility.InstantiatePrefab(tilePrefab, transform);
#else
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, transform);
#endif
                tile.transform.position = tilePosition;
                tile.transform.rotation = Quaternion.identity;
                tile.name = $"Tile ({x}, {z})";
            }
        }

        foreach (ExtraTileSelection extraTile in extratiles)
        {
            if (extraTile.tilePrefab != null)
            {
                Vector3 extraTilePosition = new Vector3(extraTile.Pos.x * tileSize, extraTile.Pos.y, extraTile.Pos.z * tileSize);
#if UNITY_EDITOR
                GameObject extra = (GameObject)PrefabUtility.InstantiatePrefab(extraTile.tilePrefab, transform);
#else
                GameObject extra = Instantiate(extraTile.tilePrefab, extraTilePosition, Quaternion.identity, transform);
#endif
                extra.transform.position = extraTilePosition;
                extra.transform.rotation = Quaternion.identity;
                extra.name = $"{extraTile.tilePrefab.name} Extra Tile ({extraTile.Pos.x}, {extraTile.Pos.z})";
            }
        }
    }

    public void ExtractTilesFromScene()
    {
        extratiles.Clear();
        int maxX = 0;
        int maxZ = 0;

        foreach (Transform child in transform)
        {
#if UNITY_EDITOR
            GameObject sourcePrefab = PrefabUtility.GetCorrespondingObjectFromSource(child.gameObject);
#else
            GameObject sourcePrefab = null;
#endif
            if (sourcePrefab == null) continue;

            Vector3 worldPos = child.position;
            int tileX = Mathf.RoundToInt(worldPos.x / tileSize);
            int tileZ = Mathf.RoundToInt(worldPos.z / tileSize);
            float tileY = worldPos.y;

            extratiles.Add(new ExtraTileSelection
            {
                tilePrefab = sourcePrefab,
                Pos = new Vector3(tileX, tileY, tileZ)
            });

            maxX = Mathf.Max(maxX, tileX);
            maxZ = Mathf.Max(maxZ, tileZ);
        }

        gridWidth = maxX + 1;
        gridHeight = maxZ + 1;
        Debug.Log($"Extracted {extratiles.Count} tiles. Grid size set to {gridWidth}x{gridHeight}.");
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

        if (GUILayout.Button("Extract From Scene"))
        {
            gridGenerator.ExtractTilesFromScene();
            EditorUtility.SetDirty(gridGenerator);
        }
    }
}
#endif
