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
    public Transform maxBound;
    public Transform minBound;

    void Awake()
    {
        // create the tiles map
        tilesmap = GetGrid();

        // create a grid
        grid = new PathFind.GridPF(tilesmap.GetLength(0), tilesmap.GetLength(1), tilesmap);
    }

    // used to highlight which tiles the character can walk to and which are blocked
    public void HighlightWalkableTiles(Vector2 startingPosition, int walkingRange)
    {        
        // get rid of any existing indicators
        foreach (GameObject gameObject in FindObjectsOfType<GameObject>())
        {
            if (gameObject.tag == "Walkable Indicator")
            {
                Destroy(gameObject);
            }
        }

        // get the square bounds of the walking range
        int startingX = Mathf.Clamp(Mathf.RoundToInt(startingPosition.x) - walkingRange, 0, tilesmap.GetLength(0) - 1);
        int endingX = Mathf.Clamp(Mathf.RoundToInt(startingPosition.x) + walkingRange, 0, tilesmap.GetLength(0) - 1);
        int startingY = Mathf.Clamp(Mathf.RoundToInt(startingPosition.y) - walkingRange, 0, tilesmap.GetLength(1) - 1);
        int endingY = Mathf.Clamp(Mathf.RoundToInt(startingPosition.y) + walkingRange, 0, tilesmap.GetLength(1) - 1);
        
        // loop through these bounds
        for (int x = startingX; x <= endingX; x++)
        {
            for (int y = startingY; y <= endingY; y++)
            {
                // get the actual walking distance from the starting position to the current tile
                int distance = PathFind.Pathfinding.GetPathDistance(grid, new PathFind.Point(Mathf.RoundToInt(startingPosition.x), Mathf.RoundToInt(startingPosition.y)), new PathFind.Point(x, y));

                // ignore any space outside of the walking range
                if (distance > walkingRange || (Mathf.Abs(x - Mathf.RoundToInt(startingPosition.x)) + Mathf.Abs(y - Mathf.RoundToInt(startingPosition.y)) > walkingRange)) { continue; }

                // a distance of >0 means the tile is walkable
                if (distance > 0)
                {
                    // create a walkable indicator on this space
                    Instantiate(walkableIndicator, new Vector2(x, y), Quaternion.identity);
                }
                else
                {
                    // create a blocked indicator on this space
                    Instantiate(blockedIndicator, new Vector2(x, y), Quaternion.identity);
                }
            }
        }
    }

    // used to set up the grid at start
    private float[,] GetGrid()
    {
        // use the positions of the bounds to set the grid size
        float[,] tilesmap = new float[Mathf.RoundToInt(maxBound.position.x), Mathf.RoundToInt(maxBound.position.y)];

        // set the values for the tiles
        for (int x = Mathf.RoundToInt(minBound.position.x); x < Mathf.RoundToInt(maxBound.position.x); x++)
        {
            for (int y = Mathf.RoundToInt(minBound.position.y); y < Mathf.RoundToInt(maxBound.position.y); y++)
            {
                //Debug.Log("mapping tile - " + x + "," + y);
                if (walkableArea.GetTile(new Vector3Int(x - 1, y - 1, 0)))
                {
                    // a value of 1 means the tile is walkable
                    tilesmap[x, y] = 1f;
                }
                if (blockedArea.GetTile(new Vector3Int(x - 1, y - 1, 0)))
                {
                    // a value of 0 means the tile is blocked
                    tilesmap[x, y] = 0f;
                }
            }
        }

        return tilesmap;
    }

    // used to build the path to pass to character pathfinding script
    public List<PathFind.Point> GetPath(PathFind.Point startPoint, PathFind.Point endPoint, int walkingRange)
    {
        return PathFind.Pathfinding.FindPath(grid, startPoint, endPoint, walkingRange);
    }

    // used to check if a point is within the bounds of the current grid to avoid errors
    public bool CheckIfPointOnGrid(Vector3Int point)
    {
        return (point.x >= -1 && point.x <= tilesmap.GetLength(0) && point.y >= -1 && point.y <= tilesmap.GetLength(1));
    }

    public bool CheckIfPointInRange(int range, Vector3 startingPosition, Vector3 endingPosition)
    {
        int distance = PathFind.Pathfinding.GetPathDistance(grid, new PathFind.Point(Mathf.RoundToInt(startingPosition.x), Mathf.RoundToInt(startingPosition.y)), new PathFind.Point(Mathf.RoundToInt(endingPosition.x), Mathf.RoundToInt(endingPosition.y)));
        return ( distance <= range);
    }

    // used to change the walkable value for a single tile in the grid 
    public void SetTileWalkable(Vector2Int tile, float newValue)
    {
        if (!CheckIfPointOnGrid((Vector3Int) tile)) { return; }
        tilesmap[tile.x, tile.y] = newValue;
        grid = new PathFind.GridPF(tilesmap.GetLength(0), tilesmap.GetLength(1), tilesmap);
    }

    public void CheckCharacterTiles()
    {
        CharacterPathfinding[] characters = FindObjectsOfType<CharacterPathfinding>();
        foreach (CharacterPathfinding character in characters)
        {
            Debug.Log("Character at " + character.transform.position);
            SetTileWalkable(new Vector2Int(Mathf.RoundToInt(character.transform.position.x), Mathf.RoundToInt(character.transform.position.y)), 0f);
            
        }
    }

    public bool CheckIfCharacterOnTile(Vector2Int tile)
    {
        if(tilesmap[tile.x, tile.y] == -1f) { return true; }
        else { return false; }
    }

    public CharacterManager GetCharacterOnTile(Vector2Int tile, CharacterManager characterLooking)
    {
        CharacterManager rightCharacter = null;
        CharacterManager[] allCharacters = FindObjectsOfType<CharacterManager>();
        foreach(CharacterManager character in allCharacters)
        {
            if(character != characterLooking && Vector2.Distance(character.transform.position, tile) < .1f)
            {
                rightCharacter = character;
                break;
            }
        }

        return rightCharacter;
    }
}
