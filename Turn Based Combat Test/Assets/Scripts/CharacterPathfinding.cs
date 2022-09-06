using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CharacterPathfinding : MonoBehaviour
{
    public float[,] tilesmap;
    public List<PathFind.Point> path;

    Rigidbody2D body;
    
    [Header ("Pathfinding References")]
    public Tilemap walkableArea;
    public Tilemap blockedArea;
    Transform topLeftBound;
    Transform topRightBound;
    Transform bottomLeftBound;
    Transform bottomRightBound;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();

        // create the tiles map
        tilesmap = GetGrid();
        // every float in the array represent the cost of passing the tile at that position.
        // use 0.0f for blocking tiles.

        // create a grid
        PathFind.Grid grid = new PathFind.Grid(tilesmap.GetLength(0), tilesmap.GetLength(1), tilesmap);

        // create source and target points
        PathFind.Point _from = new PathFind.Point(0, 0);
        PathFind.Point _to = new PathFind.Point(10, 1);

        // get path
        // path will either be a list of Points (x, y), or an empty list if no path is found.
        path = PathFind.Pathfinding.FindPath(grid, _from, _to);

        foreach(PathFind.Point point in path)
        {
            Debug.Log(point.x + ", " + point.y);
        }
    }

    private void Update()
    {
        // move through the path
        if(path.Count > 0)
        {
            if(Vector2.Distance(this.transform.position, new Vector2(path[0].x, path[0].y)) >= .1f)
            {
                body.velocity = new Vector2(path[0].x - this.transform.position.x, path[0].y - this.transform.position.y).normalized * 3f;
            }
            else
            {
                body.velocity = Vector2.zero;
                path.RemoveAt(0);
            }
        }
        else
        {
            // make sure we're exactly on the space we want to end on
            this.transform.position = new Vector2(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y));
        }
        
    }

    private float[,] GetGrid()
    {    
        // use the indexes of the tiles to set the grid size
        float[,] tilesmap = new float[walkableArea.cellBounds.size.x, walkableArea.cellBounds.size.y];

        // set the values for the tiles
        for(int x = walkableArea.cellBounds.xMin; x <= walkableArea.cellBounds.xMax; x++)
        {
            for (int y = walkableArea.cellBounds.yMin; y <= walkableArea.cellBounds.yMax; y++)
            {
                if(walkableArea.GetTile(new Vector3Int(x, y, 0))){
                    Debug.Log(x + ", " + y + " - Walkable");
                    tilesmap[x + 1, y + 1] = 1f;
                }
            }
        }

        for (int x = blockedArea.cellBounds.xMin; x <= blockedArea.cellBounds.xMax; x++)
        {
            for (int y = blockedArea.cellBounds.yMin; y <= blockedArea.cellBounds.yMax; y++)
            {
                if (blockedArea.GetTile(new Vector3Int(x, y, 0)))
                {
                    Debug.Log(x + ", " + y + " - Not Walkable");
                    tilesmap[x + 1, y + 1] = 0f;
                }
            }
        }

        return tilesmap;
    }
}
