using UnityEngine;
using UnityEngine.Tilemaps;

// Turns the ground tilemap into a grid of terrain data (Rain World style).
// The tilemap is only for painting the level; bodies collide with this data, not with physics colliders.
public class Room : MonoBehaviour
{
    public enum Shape { Air, Solid }

    [SerializeField] Tilemap groundTilemap;

    public float TileSize { get; private set; }

    Vector2 origin;
    BoundsInt bounds;
    Shape[,] shapes;

    void Awake()
    {
        TileSize = groundTilemap.layoutGrid.cellSize.x;
        origin = groundTilemap.CellToWorld(Vector3Int.zero);

        groundTilemap.CompressBounds();
        bounds = groundTilemap.cellBounds;
        shapes = new Shape[bounds.size.x, bounds.size.y];

        foreach (Vector3Int cell in bounds.allPositionsWithin)
        {
            if (groundTilemap.HasTile(cell)) shapes[cell.x - bounds.xMin, cell.y - bounds.yMin] = Shape.Solid;
        }
    }

    public Shape GetShape(int x, int y)
    {
        bool inBounds = x >= bounds.xMin && x < bounds.xMax && y >= bounds.yMin && y < bounds.yMax;
        return inBounds ? shapes[x - bounds.xMin, y - bounds.yMin] : Shape.Air;
    }

    public bool IsSolid(int x, int y)
    {
        return GetShape(x, y) == Shape.Solid;
    }

    // World position <-> tile coordinates
    public int TileX(float worldX) => Mathf.FloorToInt((worldX - origin.x) / TileSize);
    public int TileY(float worldY) => Mathf.FloorToInt((worldY - origin.y) / TileSize);
    public float TileLeft(int x) => origin.x + x * TileSize;
    public float TileBottom(int y) => origin.y + y * TileSize;
}
