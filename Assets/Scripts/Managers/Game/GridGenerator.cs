using UnityEngine;
using System.Collections.Generic;
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
    public GameObject airPrefab;

    public int gridWidth = 5;
    public int gridHeight = 5;
    public int gridLength = 1;

    public float tileSize = 1f;

    public List<ExtraTileSelection> extratiles = new List<ExtraTileSelection>();
    public List<ExtraTileSelection> airTiles = new List<ExtraTileSelection>();

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

        HashSet<Vector3> extraPositions = new HashSet<Vector3>();
        foreach (var extra in extratiles)
            extraPositions.Add(extra.Pos);
        foreach (var air in airTiles)
            extraPositions.Add(air.Pos);

        // Generate main grid
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                for (int y = 0; y < gridLength; y++)
                {
                    Vector3 pos = new Vector3(x, y, z);
                    if (extraPositions.Contains(pos)) continue;

                    Vector3 worldPos = new Vector3(x * tileSize, y, z * tileSize);
#if UNITY_EDITOR
                    GameObject tile = (GameObject)PrefabUtility.InstantiatePrefab(tilePrefab, Objects.transform);
#else
                    GameObject tile = Instantiate(tilePrefab, worldPos, Quaternion.identity, Objects.transform);
#endif
                    tile.transform.position = worldPos;
                    tile.transform.rotation = Quaternion.identity;
                    tile.name = $"Tile ({x}, {y}, {z})";
                }
            }
        }

        // Generate extra tiles
        foreach (var extra in extratiles)
        {
            if (extra.tilePrefab == null) continue;

            Vector3 worldPos = new Vector3(extra.Pos.x * tileSize, extra.Pos.y, extra.Pos.z * tileSize);
#if UNITY_EDITOR
            GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(extra.tilePrefab, ExtraObjects.transform);
#else
            GameObject go = Instantiate(extra.tilePrefab, worldPos, Quaternion.identity, ExtraObjects.transform);
#endif
            go.transform.position = worldPos;
            go.transform.rotation = Quaternion.identity;
            string firstWord = extra.tilePrefab.name.Split(' ')[0];
            go.name = $"{firstWord} Extra Tile ({extra.Pos.x}, {extra.Pos.y}, {extra.Pos.z})";
        }

        // Generate air tiles
        foreach (var air in airTiles)
        {
            if (air.tilePrefab == null) continue;

            Vector3 worldPos = new Vector3(air.Pos.x * tileSize, air.Pos.y, air.Pos.z * tileSize);
#if UNITY_EDITOR
            GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(air.tilePrefab, ExtraObjects.transform);
#else
            GameObject go = Instantiate(air.tilePrefab, worldPos, Quaternion.identity, ExtraObjects.transform);
#endif
            go.transform.position = worldPos;
            go.transform.rotation = Quaternion.identity;
            go.name = $"Air Tile ({air.Pos.x}, {air.Pos.y}, {air.Pos.z})";
        }
    }

    public void ExtractTilesFromScene()
    {
        extratiles.Clear();
        airTiles.Clear();

        if (ExtraObjects == null || Objects == null)
        {
            Debug.LogWarning("No Objects or ExtraObjects found.");
            return;
        }

        HashSet<Vector3Int> gridAligned = new HashSet<Vector3Int>();
        int maxX = 0, maxY = 0, maxZ = 0;

        List<Transform> allTiles = new List<Transform>();
        allTiles.AddRange(Objects.transform.Cast<Transform>());
        allTiles.AddRange(ExtraObjects.transform.Cast<Transform>());

        foreach (Transform tile in allTiles)
        {
#if UNITY_EDITOR
            GameObject prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(tile.gameObject);
#else
            GameObject prefab = tilePrefab;
#endif
            Vector3 pos = tile.position;

            int x = Mathf.RoundToInt(pos.x / tileSize);
            int y = Mathf.RoundToInt(pos.y);
            int z = Mathf.RoundToInt(pos.z / tileSize);

            Vector3 aligned = new Vector3(x * tileSize, y, z * tileSize);
            Vector3Int gridPos = new Vector3Int(x, y, z);
            bool isAligned = Vector3.Distance(pos, aligned) < 0.01f;
            bool isMainTile = prefab == tilePrefab && isAligned && tile.rotation == Quaternion.identity;

            if (isMainTile)
            {
                gridAligned.Add(gridPos);
                maxX = Mathf.Max(maxX, x + 1);
                maxY = Mathf.Max(maxY, y + 1);
                maxZ = Mathf.Max(maxZ, z + 1);
            }
            else
            {
                extratiles.Add(new ExtraTileSelection { tilePrefab = prefab, Pos = gridPos });
                maxX = Mathf.Max(maxX, x + 1);
                maxY = Mathf.Max(maxY, y + 1);
                maxZ = Mathf.Max(maxZ, z + 1);
            }
        }

        gridWidth = maxX;
        gridLength = maxY;
        gridHeight = maxZ;

        // Detect air blocks
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridLength; y++)
            {
                for (int z = 0; z < gridHeight; z++)
                {
                    Vector3Int pos = new Vector3Int(x, y, z);
                    bool exists = gridAligned.Contains(pos) || extratiles.Exists(t => t.Pos == (Vector3)pos);

                    if (!exists)
                    {
                        airTiles.Add(new ExtraTileSelection
                        {
                            tilePrefab = airPrefab,
                            Pos = new Vector3(x, y, z)
                        });
                    }
                }
            }
        }

        Debug.Log($"Extracted {extratiles.Count} extra tiles.");
        Debug.Log($"Extracted {airTiles.Count} air tiles.");
    }

    public void ClearGrid()
    {
        void ClearChildren(Transform t)
        {
            for (int i = t.childCount - 1; i >= 0; i--)
            {
#if UNITY_EDITOR
                DestroyImmediate(t.GetChild(i).gameObject);
#else
                Destroy(t.GetChild(i).gameObject);
#endif
            }
        }

        if (Objects != null) ClearChildren(Objects.transform);
        if (ExtraObjects != null) ClearChildren(ExtraObjects.transform);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(GridGenerator))]
public class GridGeneratorEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridGenerator g = (GridGenerator)target;

        if (GUILayout.Button("Generate Grid"))
        {
            g.GenerateGrid();
            EditorUtility.SetDirty(g);
        }

        if (GUILayout.Button("Clear Grid"))
        {
            g.ClearGrid();
            EditorUtility.SetDirty(g);
        }

        if (GUILayout.Button("Extract From Scene"))
        {
            g.ExtractTilesFromScene();
            EditorUtility.SetDirty(g);
        }
    }
}
#endif
