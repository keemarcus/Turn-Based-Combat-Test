using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CharacterPathfinding : MonoBehaviour
{
    public float[,] tilesmap;
    public List<PathFind.Point> path;

    Rigidbody2D body;

    PathFind.GridPF grid;
    PathFind.Point _from;
    PathFind.Point _to;

    Animator animator;

    [Header ("Pathfinding References")]
    public Tilemap walkableArea;
    public Tilemap blockedArea;
    public GameObject walkableIndicator;
    public GameObject blockedIndicator;
    public int walkingRange = 5;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // create the tiles map
        tilesmap = GetGrid();

        // create a grid
        grid = new PathFind.GridPF(tilesmap.GetLength(0), tilesmap.GetLength(1), tilesmap);

        HighlightWalkableTiles();
    }

    private void MoveToTile(int targetX, int targetY)
    {
        _from = new PathFind.Point(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y));
        _to = new PathFind.Point(targetX, targetY);
        path = PathFind.Pathfinding.FindPath(grid, _from, _to, walkingRange);
    }

    private void HighlightWalkableTiles()
    {
        foreach(GameObject gameObject in FindObjectsOfType<GameObject>())
        {
            if(gameObject.tag == "Walkable Indicator")
            {
                Destroy(gameObject);
            }
        }

        for (int x = 0; x < tilesmap.GetLength(0); x++){
            for (int y = 0; y < tilesmap.GetLength(1); y++)
            {
                //Debug.Log("TIle - " + x + "," + y);
                int distance = PathFind.Pathfinding.GetPathDistance(grid, new PathFind.Point(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y)), new PathFind.Point(x, y));
                if(distance <= walkingRange)
                {
                    if (distance < 0)
                    {
                        if(Mathf.Abs(x - Mathf.RoundToInt(this.transform.position.x)) + Mathf.Abs(y - Mathf.RoundToInt(this.transform.position.y)) <= walkingRange){ Instantiate(blockedIndicator, new Vector2(x, y), Quaternion.identity); }
                    }
                    else
                    {
                        Instantiate(walkableIndicator, new Vector2(x, y), Quaternion.identity);
                    }
                    
                }
            }
        }
    }

    private void Update()
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var noZ = new Vector3(pos.x, pos.y);
        Vector3Int mouseCell = FindObjectOfType<Grid>().WorldToCell(noZ);
        
        if (Input.GetMouseButtonUp(0))
        {
            if (mouseCell.x >= -1 && mouseCell.x <= tilesmap.GetLength(0) && mouseCell.y >= -1 && mouseCell.y <= tilesmap.GetLength(1))
            {
                MoveToTile(mouseCell.x + 1, mouseCell.y + 1);
            }
        }

        // move through the path
        if(path != null && path.Count > 0)
        {
            if(Vector2.Distance(this.transform.position, new Vector2(path[0].x, path[0].y)) >= .1f)
            {
                body.velocity = new Vector2(path[0].x - this.transform.position.x, path[0].y - this.transform.position.y).normalized * 3f;
            }
            else
            {
                //body.velocity = Vector2.zero;
                path.RemoveAt(0);
            }
        }
        else
        {
            // make sure we're exactly on the space we want to end on
            this.transform.position = new Vector2(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y));

            // make sure velocity is zeroed and update the animator
            body.velocity = Vector2.zero;

            HighlightWalkableTiles();
        }
        
        // update the animator
        if(body.velocity == Vector2.zero)
        {
            animator.SetBool("Walking", false);
            //animator.SetFloat("X", 0f);
            //animator.SetFloat("Y", 0f);
        } else
        {
            animator.SetBool("Walking", true);
            animator.SetFloat("X", body.velocity.normalized.x);
            animator.SetFloat("Y", body.velocity.normalized.y);
        }

    }

    private float[,] GetGrid()
    {    
        // use the indexes of the tiles to set the grid size
        float[,] tilesmap = new float[walkableArea.cellBounds.size.x - 2, walkableArea.cellBounds.size.y - 1];

        // set the values for the tiles
        for(int x = walkableArea.cellBounds.xMin; x <= walkableArea.cellBounds.xMax - 1; x++)
        {
            for (int y = walkableArea.cellBounds.yMin; y <= walkableArea.cellBounds.yMax - 1; y++)
            {
                if(walkableArea.GetTile(new Vector3Int(x, y, 0))){
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
}
