using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CharacterPathfinding : MonoBehaviour
{
    public List<PathFind.Point> path;

    Rigidbody2D body;

    PathFind.Point _from;
    PathFind.Point _to;

    Animator animator;

    [Header("Pathfinding References")]
    //public Tilemap walkableArea;
    //public Tilemap blockedArea;
    //public GameObject walkableIndicator;
    //public GameObject blockedIndicator;
    public GridManager gridManager;
    public int walkingRange = 5;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        gridManager = FindObjectOfType<GridManager>();

        gridManager.SetTileWalkable(new Vector2Int(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y)), false);
        gridManager.HighlightWalkableTiles(this.transform.position, walkingRange);
    }

    private void MoveToTile(int targetX, int targetY)
    {
        _from = new PathFind.Point(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y));
        _to = new PathFind.Point(targetX, targetY);
        path = gridManager.GetPath(_from, _to, walkingRange);
    }

    

    private void Update()
    {
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var noZ = new Vector3(pos.x, pos.y);
        Vector3Int mouseCell = FindObjectOfType<Grid>().WorldToCell(noZ);
        
        if (Input.GetMouseButtonUp(0))
        {
            if (gridManager.CheckIfPointOnGrid(mouseCell))
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

            gridManager.SetTileWalkable(new Vector2Int(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y)), false);
            gridManager.HighlightWalkableTiles(this.transform.position, walkingRange);
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
}
