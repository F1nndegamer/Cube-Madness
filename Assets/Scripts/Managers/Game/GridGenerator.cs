using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class ExtraTileSelection
{
    public int tilePrefab = 0;
    public Vector3 Pos = Vector3.zero;

    public Vector3 BlockPos = Vector3.zero;
    public Vector3 MovePosition = Vector3.zero;
}

public class GridGenerator : MonoBehaviour
{
    public Blueprint blueprint;

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
    [Header("Block Prefabs")]
    public GameObject pressureplatePrefab;
    public GameObject breakPrefab;
    public GameObject endPrefab;
    public GameObject playerPrefab;
    public GameObject deactplayerPrefab;
    public GameObject wallXPrefab;
    public GameObject wallZPrefab;
    public GameObject TimerPrefab;
    public GameObject MoveablePrefab;
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

        foreach (var extra in extratiles)
        {
            GameObject prefab = null;
            if (extra.tilePrefab == 0) continue;

            switch (extra.tilePrefab)
            {
                case 1: prefab = pressureplatePrefab; break;
                case 2: prefab = breakPrefab; break;
                case 3: prefab = endPrefab; break;
                case 4: prefab = playerPrefab; break;
                case 5: prefab = deactplayerPrefab; break;
                case 6: prefab = wallXPrefab; break;
                case 7: prefab = wallZPrefab; break;
                case 8: prefab = TimerPrefab; break;
                case 9: prefab = MoveablePrefab; break;
            }

            Vector3 worldPos = new Vector3(extra.Pos.x * tileSize, extra.Pos.y, extra.Pos.z * tileSize);
            Vector3 offset = GetPlacementOffset(extra.tilePrefab);

#if UNITY_EDITOR
            GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(prefab, ExtraObjects.transform);
#else
        GameObject go = Instantiate(prefab, worldPos + offset, Quaternion.identity, ExtraObjects.transform);
#endif

            go.transform.position = worldPos + offset;

            if (extra.tilePrefab == 6)
                go.transform.rotation = Quaternion.Euler(0, 90, 0);
            else
                go.transform.rotation = Quaternion.identity;

            string firstWord = prefab.name.Split(' ')[0];
            go.name = $"{firstWord} Extra Tile ({extra.Pos.x}, {extra.Pos.y}, {extra.Pos.z})";
        }

        foreach (var air in airTiles)
        {
            if (air.tilePrefab == 0) continue;
            Vector3 worldPos = new Vector3(air.Pos.x * tileSize, air.Pos.y, air.Pos.z * tileSize);
        }
        SetExtraTileMoveData();
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

        HashSet<Vector3Int> occupiedPositions = new HashSet<Vector3Int>();

        List<Transform> allTiles = new List<Transform>();
        allTiles.AddRange(Objects.transform.Cast<Transform>());
        allTiles.AddRange(ExtraObjects.transform.Cast<Transform>());

        int maxX = 0, maxY = 0, maxZ = 0;

        foreach (Transform tile in allTiles)
        {
#if UNITY_EDITOR
            GameObject prefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(tile.gameObject);
#else
        GameObject prefab = tilePrefab;
#endif
            if (prefab == null) continue;

            Vector3 pos = tile.position;
            int index = GetPrefabIndex(prefab);
            Vector3 placementOffset = GetPlacementOffset(index);
            Vector3 adjustedPos = pos - placementOffset;

            int x = Mathf.RoundToInt(adjustedPos.x / tileSize);
            int y = Mathf.RoundToInt(adjustedPos.y);
            int z = Mathf.RoundToInt(adjustedPos.z / tileSize);
            Vector3Int gridPos = new Vector3Int(x, y, z);
            occupiedPositions.Add(gridPos);

            bool isMainTile = prefab == tilePrefab && tile.rotation == Quaternion.identity;

            if (!isMainTile)
            {
                index = GetPrefabIndex(prefab);
                if (index > 0)
                {
                    extratiles.Add(new ExtraTileSelection { tilePrefab = index, Pos = new Vector3(x, y, z) });
                }
            }

            maxX = Mathf.Max(maxX, x + 1);
            maxY = Mathf.Max(maxY, y + 1);
            maxZ = Mathf.Max(maxZ, z + 1);
        }

        for (int x = 0; x < maxX; x++)
        {
            for (int y = 0; y < maxY; y++)
            {
                for (int z = 0; z < maxZ; z++)
                {
                    Vector3Int pos = new Vector3Int(x, y, z);
                    if (!occupiedPositions.Contains(pos))
                    {
                        airTiles.Add(new ExtraTileSelection { tilePrefab = 0, Pos = new Vector3(x, y, z) });
                    }
                }
            }
        }

        gridWidth = maxX;
        gridLength = maxY;
        gridHeight = maxZ;

        Debug.Log($"Extracted {extratiles.Count} extra tiles.");
        Debug.Log($"Extracted {airTiles.Count} air tiles.");
    }


    private int GetPrefabIndex(GameObject prefab)
    {
        if (prefab == pressureplatePrefab) return 1;
        if (prefab == breakPrefab) return 2;
        if (prefab == endPrefab) return 3;
        if (prefab == playerPrefab) return 4;
        if (prefab == deactplayerPrefab) return 5;
        if (prefab == wallXPrefab) return 6;
        if (prefab == wallZPrefab) return 7;
        if (prefab == TimerPrefab) return 8;
        if (prefab == MoveablePrefab) return 9;
        return -1;
    }

    private Vector3 GetPlacementOffset(int tilePrefab)
    {
        switch (tilePrefab)
        {
            case 6: return new Vector3(0, -0.2f, -tileSize / 2f);
            case 7: return new Vector3(-tileSize / 2f, -0.2f, 0);
            default: return Vector3.zero;
        }
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
    public void Reset()
    {
        GameObject extratiles = GameObject.Find("ExtraObjects");
        if (extratiles != null)
        {
            ExtraObjects = extratiles;
        }

        GameObject objects = GameObject.Find("Objects");
        if (objects != null)
        {
            Objects = objects;
        }
    }
    private void SetExtraTileMoveData()
    {
        ExtraTileTrigger[] triggers = FindObjectsByType<ExtraTileTrigger>(FindObjectsSortMode.None);
        BreakableBlock[] breakers = FindObjectsByType<BreakableBlock>(FindObjectsSortMode.None);
        foreach (var trigger in triggers)
        {
            Vector3 triggerGridPos = new Vector3(
                Mathf.RoundToInt(trigger.transform.position.x / tileSize),
                Mathf.RoundToInt(trigger.transform.position.y),
                Mathf.RoundToInt(trigger.transform.position.z / tileSize)
            );
            Debug.Log($"Processing trigger at {triggerGridPos}");
            foreach (var tile in extratiles)
            {
                if (tile.Pos == triggerGridPos)
                {
                    if (tile.tilePrefab == 1)
                    {
                        Vector3 worldBlockPos = new Vector3(
                            tile.BlockPos.x * tileSize,
                            tile.BlockPos.y,
                            tile.BlockPos.z * tileSize
                        );

                        GameObject found = FindObjectAtPosition(worldBlockPos, Objects.transform);
                        if (found != null)
                        {
                            if (trigger.objectToMove.Count == 0)
                                trigger.objectToMove.Add(found.transform);
                            else
                                trigger.objectToMove[0] = found.transform;
                        }
                        else
                        {
                            Debug.LogWarning($"No object found at {worldBlockPos} for tile at {tile.Pos}");
                        }

                        if (trigger.targetPosition.Count == 0)
                            trigger.targetPosition.Add(tile.MovePosition);
                        else
                            trigger.targetPosition[0] = tile.MovePosition;

#if UNITY_EDITOR
                        Debug.Log($"Linked tile at {tile.Pos} to object at {tile.BlockPos} -> {found?.name}");
#endif
                    }
                    else if (tile.tilePrefab != 1)
                    {
                        Debug.Log($"Tile at {tile.Pos} is not a valid type.");
                    }
                    else
                    {
                        Debug.Log($"Tile at {tile.Pos} does not match trigger at {triggerGridPos}.");
                    }
                }
            }
        }

        foreach (var breaker in breakers)
        {
            Vector3 triggerGridPos = new Vector3(
                Mathf.RoundToInt(breaker.transform.position.x / tileSize),
                Mathf.RoundToInt(breaker.transform.position.y),
                Mathf.RoundToInt(breaker.transform.position.z / tileSize)
            );
            Debug.Log($"1Processing breaker at {triggerGridPos}");
            foreach (var tile in extratiles)
            {
                if (tile.Pos == triggerGridPos)
                {
                    if (tile.tilePrefab == 1)
                    {
                        Vector3 worldBlockPos = new Vector3(
                            tile.BlockPos.x * tileSize,
                            tile.BlockPos.y,
                            tile.BlockPos.z * tileSize
                        );

                        GameObject found = FindObjectAtPosition(worldBlockPos, Objects.transform);
                        if (found != null)
                        {
                            if (breaker.blocks.Count == 0)
                                breaker.blocks.Add(found.transform);
                            else
                                breaker.blocks[0] = found.transform;
                        }
                        else
                        {
                            Debug.LogWarning($"1No object found at {worldBlockPos} for tile at {tile.Pos}");
                        }
#if UNITY_EDITOR
                        Debug.Log($"1Linked tile at {tile.Pos} to object at {tile.BlockPos} -> {found?.name}");
#endif
                    }
                    else if (tile.tilePrefab != 1)
                    {
                        Debug.Log($"1Tile at {tile.Pos} is not a valid type.");
                    }
                    else
                    {
                        Debug.Log($"11Tile at {tile.Pos} does not match trigger at {triggerGridPos}.");
                    }
                }
            }
        }

    }
    private GameObject FindObjectAtPosition(Vector3 position, Transform parent)
    {
        foreach (Transform child in parent)
        {
            if (Vector3.Distance(child.position, position) < 0.01f)
            {
                return child.gameObject;
            }
        }
        return null;
    }

    public void GetExtraTileMoveData(GridGenerator grid)
    {
        ExtraTileTrigger[] triggers = GameObject.FindObjectsByType<ExtraTileTrigger>(FindObjectsSortMode.None);

        foreach (var trigger in triggers)
        {
            Vector3 triggerGridPos = new Vector3(
                Mathf.RoundToInt(trigger.transform.position.x / grid.tileSize),
                Mathf.RoundToInt(trigger.transform.position.y),
                Mathf.RoundToInt(trigger.transform.position.z / grid.tileSize)
            );

            foreach (var tile in grid.extratiles)
            {
                if (tile.Pos == triggerGridPos && (tile.tilePrefab == 1 || tile.tilePrefab == 2))
                {
                    if (trigger.objectToMove.Count > 0 && trigger.targetPosition.Count > 0)
                    {
                        tile.BlockPos = trigger.objectToMove[0].position;
                        tile.MovePosition = trigger.targetPosition[0];

                        Debug.Log($"Set BlockPos and MovePosition for tile at {tile.Pos} using trigger at {triggerGridPos}");
                    }
                    else
                    {
                        Debug.LogWarning($"Trigger at {triggerGridPos} has no move targets.");
                    }
                }
            }
        }
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
            g.GetExtraTileMoveData(g);
            EditorUtility.SetDirty(g);
        }
    }
}
#endif
