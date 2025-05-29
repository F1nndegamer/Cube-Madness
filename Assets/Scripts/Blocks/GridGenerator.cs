using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;
using System.Linq;



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
    public int gridLength = 1;

    public float tileSize = 1f;
    public List<ExtraTileSelection> extratiles = new List<ExtraTileSelection>();
    public GameObject ExtraObjects;
    public GameObject Objects;
    public void GenerateGrid()
    {
        if (ExtraObjects == null) ExtraObjects = new GameObject("ExtraObjects");
        if (Objects == null) Objects = new GameObject("Objects");
        ExtraObjects.transform.SetParent(transform);
        Objects.transform.SetParent(transform);
        if (tilePrefab == null)
        {
            Debug.LogWarning("Assign a Tile Prefab");
            return;
        }

        ClearGrid();

        HashSet<Vector3> extraTilePositions = new HashSet<Vector3>();
        foreach (var extra in extratiles)
        {
            extraTilePositions.Add(new Vector3(extra.Pos.x, extra.Pos.y, extra.Pos.z));
        }

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                for (int y = 0; y < gridLength; y++)
                {
                    if (extraTilePositions.Contains(new Vector3(x, y, z))) continue;

                    Vector3 tilePosition = new Vector3(x * tileSize, 0, z * tileSize);
#if UNITY_EDITOR
                    GameObject tile = (GameObject)PrefabUtility.InstantiatePrefab(tilePrefab, Objects.transform);
#else
                GameObject tile = Instantiate(tilePrefab, tilePosition, Quaternion.identity, Objects.transform);
#endif
                    tile.transform.position = tilePosition;
                    tile.transform.rotation = Quaternion.identity;
                    tile.name = $"Tile ({x}, {y}, {z})";
                }
            }
        }

        foreach (ExtraTileSelection extraTile in extratiles)
        {
            if (extraTile.tilePrefab != null)
            {
                Vector3 extraTilePosition = new Vector3(extraTile.Pos.x * tileSize, extraTile.Pos.y, extraTile.Pos.z * tileSize);
#if UNITY_EDITOR
                GameObject extra = (GameObject)PrefabUtility.InstantiatePrefab(extraTile.tilePrefab, ExtraObjects.transform);
#else
                GameObject extra = Instantiate(extraTile.tilePrefab, extraTilePosition, Quaternion.identity, ExtraObjects.transform);
#endif
                extra.transform.position = extraTilePosition;
                extra.transform.rotation = Quaternion.identity;
                string firstWord = extraTile.tilePrefab.name.Split(' ')[0];
                extra.name = $"{firstWord} Extra Tile ({extraTile.Pos.x}, {extraTile.Pos.y}, {extraTile.Pos.z})";

            }
        }
    }

    public void ExtractTilesFromScene()
    {
        extratiles.Clear();

        if (ExtraObjects == null || Objects == null)
        {
            Debug.LogWarning("No Objects or ExtraObjects found.");
            return;
        }

        HashSet<Vector3Int> gridAlignedPositions = new HashSet<Vector3Int>();
        int maxX = 0, maxY = 0, maxZ = 0;

        List<Transform> allTiles = new List<Transform>();
        allTiles.AddRange(Objects.transform.Cast<Transform>());
        allTiles.AddRange(ExtraObjects.transform.Cast<Transform>());

        foreach (Transform tile in allTiles)
        {
            GameObject prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(tile.gameObject);
            Vector3 worldPos = tile.position;

            int x = Mathf.RoundToInt(worldPos.x / tileSize);
            int y = Mathf.RoundToInt(worldPos.y);
            int z = Mathf.RoundToInt(worldPos.z / tileSize);
            Vector3 gridWorldPos = new Vector3(x * tileSize, y, z * tileSize);

            bool isAligned = Vector3.Distance(worldPos, gridWorldPos) < 0.01f;
            bool isMainTile = prefab == tilePrefab && isAligned && tile.rotation == Quaternion.identity;

            Vector3Int gridCoord = new Vector3Int(x, y, z);

            if (isMainTile)
            {
                gridAlignedPositions.Add(gridCoord);
                maxX = Mathf.Max(maxX, x + 1);
                maxY = Mathf.Max(maxY, y + 1);
                maxZ = Mathf.Max(maxZ, z + 1);
            }
            else
            {
                extratiles.Add(new ExtraTileSelection
                {
                    tilePrefab = prefab,
                    Pos = new Vector3(x, y, z)
                });

                maxX = Mathf.Max(maxX, x + 1);
                maxY = Mathf.Max(maxY, y + 1);
                maxZ = Mathf.Max(maxZ, z + 1);
            }
        }

        gridWidth = maxX;
        gridLength = maxY;
        gridHeight = maxZ;

        Debug.Log($"Extracted {extratiles.Count} extra tiles.");
        Debug.Log($"New Grid Size: Width={gridWidth}, Length={gridLength}, Height={gridHeight}");
    }






    public void ClearGrid()
    {
        for (int i = Objects.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(Objects.transform.GetChild(i).gameObject);
        }
        for (int i = ExtraObjects.transform.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(ExtraObjects.transform.GetChild(i).gameObject);
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
