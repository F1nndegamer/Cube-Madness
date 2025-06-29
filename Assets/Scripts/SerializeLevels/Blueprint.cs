using UnityEngine;

[System.Serializable]
public class Blueprint
{
    public enum TileType
    {
        Air = 0,
    }
    [System.Serializable]
    public struct Tile
    {
        public Vector3 position;
        public TileType type;
    }
    public int width;
    public int height;
    public int length;
    public float tileSize = 1;
    public Vector3[] airTiles;
    public Tile[] extraTiles;
}
