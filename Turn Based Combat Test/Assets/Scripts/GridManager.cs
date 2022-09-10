using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [Header("Pathfinding References")]
    private float[,] tilesmap;
    PathFind.GridPF grid;
    public Tilemap walkableArea;
    public Tilemap blockedArea;
    public GameObject walkableIndicator;
    public GameObject blockedIndicator;
    // Start is called before the first frame update
    void Start()
    {
        // create the tiles map
        tilesmap = GetGrid();

        // create a grid
        grid = new PathFind.GridPF(tilesmap.GetLength(0), tilesmap.GetLength(1), tilesmap);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HighlightWalkableTiles(Vector2 startingPosition, int walkingRange)
    {        
        foreach (GameObject gameObject in FindObjectsOfType<GameObject>())
        {
            if (gameObject.tag == "Walkable Indicator")
            {
                Destroy(gameObject);
            }
        }

        for (int x = 0; x < tilesmap.GetLength(0); x++)
        {
            for (int y = 0; y < tilesmap.GetLength(1); y++)
            {
                //Debug.Log("TIle - " + x + "," + y);
                int distance = PathFind.Pathfinding.GetPathDistance(grid, new PathFind.Point(Mathf.RoundToInt(startingPosition.x), Mathf.RoundToInt(startingPosition.y)), new PathFind.Point(x, y));
                if (distance <= walkingRange)
                {
                    if (distance <= 0)
                    {
                        if (Mathf.Abs(x - Mathf.RoundToInt(startingPosition.x)) + Mathf.Abs(y - Mathf.RoundToInt(startingPosition.y)) <= walkingRange) { Instantiate(blockedIndicator, new Vector2(x, y), Quaternion.identity); }
                    }
                    else
                    {
                        Instantiate(walkableIndicator, new Vector2(x, y), Quaternion.identity);
                    }

                }
            }
        }
    }

    private float[,] GetGrid()
    {
        // use the indexes of the tiles to set the grid size
        float[,] tilesmap = new float[walkableArea.cellBounds.size.x - 2, walkableArea.cellBounds.size.y - 1];

        // set the values for the tiles
        for (int x = walkableArea.cellBounds.xMin; x <= walkableArea.cellBounds.xMax - 1; x++)
        {
            for (int y = walkableArea.cellBounds.yMin; y <= walkableArea.cellBounds.yMax - 1; y++)
            {
                if (walkableArea.GetTile(new Vector3Int(x, y, 0)))
                {
                    tilesmap[x + 1, y + 1] = 1f;
                }
                if (blockedArea.GetTile(new Vector3Int(x, y, 0)))
                {
                    tilesmap[x + 1, y + 1] = 0f;
                }
            }
        }

        return tilesmap;
    }

    public List<PathFind.Point> GetPath(PathFind.Point startPoint, PathFind.Point endPoint, int walkingRange)
    {
        return PathFind.Pathfinding.FindPath(grid, startPoint, endPoint, walkingRange);
    }

    public bool CheckIfPointOnGrid(Vector3Int point)
    {
        return (point.x >= -1 && point.x <= tilesmap.GetLength(0) && point.y >= -1 && point.y <= tilesmap.GetLength(1));
    }

    public void SetTileWalkable(Vector2Int tile, bool walkable)
    {
        Debug.Log("Tile - " + tile + " Walkable - " + walkable);
        if (walkable)
        {
            tilesmap[tile.x, tile.y] = 1f;
        } else
        {
            tilesmap[tile.x, tile.y] = 0f;
        }
    }
}
