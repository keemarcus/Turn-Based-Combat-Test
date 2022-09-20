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

    public bool turn;
    private bool doneMoving;
    public bool interacting;
    public int attackRange = 1;
    public int maxHealth = 50;
    public int health;
    public int attackDamage = 10;

    public TurnManager turnManager;

    [Header("Pathfinding References")]
    public GridManager gridManager;
    public int walkingRange = 5;
    public int movesLeft;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        gridManager = FindObjectOfType<GridManager>();
        turnManager = FindObjectOfType<Camera>().GetComponentInChildren<TurnManager>();

        health = maxHealth;

        interacting = false;
        doneMoving = false;
    }

    private void MoveToTile(int targetX, int targetY)
    {
        _from = new PathFind.Point(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y));
        _to = new PathFind.Point(targetX, targetY);
        path = gridManager.GetPath(_from, _to, walkingRange);

        // update the grid
        gridManager.SetTileWalkable(new Vector2Int(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y)), 1f);
        gridManager.SetTileWalkable(new Vector2Int(targetX, targetY), -1f);

        doneMoving = false;
    }

    

    private void Update()
    {
        if (!turn) { return; }
        
        // get the mouse position on the grid
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var noZ = new Vector3(pos.x, pos.y);
        Vector3Int mouseCell = FindObjectOfType<Grid>().WorldToCell(noZ);
        
        // only register clicks if we're not already moving
        if (Input.GetMouseButtonUp(1) && body.velocity == Vector2.zero)
        {    
            if (gridManager.CheckIfPointOnGrid(mouseCell))
            {
                if(gridManager.CheckIfCharacterOnTile(new Vector2Int(Mathf.RoundToInt(noZ.x), Mathf.RoundToInt(noZ.y))) && (Mathf.RoundToInt(Mathf.Abs(this.transform.position.x - noZ.x)) + Mathf.RoundToInt(Mathf.Abs(this.transform.position.y - noZ.y))) <= attackRange)
                {
                    CharacterPathfinding enemy = gridManager.GetCharacterOnTile(new Vector2Int(Mathf.RoundToInt(noZ.x), Mathf.RoundToInt(noZ.y)));
                    if(enemy == null)
                    {
                        Debug.Log("No character found");
                    }
                    else
                    {
                        if (Attack(enemy))
                        {
                            Debug.Log("Killed " + enemy.gameObject.name);
                        }
                        else
                        {
                            Debug.Log(enemy.gameObject.name + " took " + this.attackDamage + " damage");
                        }
                    }
                    
                }else if(gridManager.CheckIfPointInRange(movesLeft, this.transform.position, noZ))
                {
                    MoveToTile(mouseCell.x + 1, mouseCell.y + 1);
                }
            }
        }

        // move through the path if we have one
        if(path != null && path.Count > 0)
        {
            interacting = true;

            if(Vector2.Distance(this.transform.position, new Vector2(path[0].x, path[0].y)) >= .1f)
            {
                body.velocity = new Vector2(path[0].x - this.transform.position.x, path[0].y - this.transform.position.y).normalized * 3f;
            }
            else
            {
                path.RemoveAt(0);
                // remove a space from our moves left
                UpdateMovesLeft(1);
            }
        }
        else if(!doneMoving)// we're at the end of the path
        {
            // make sure we're exactly on the space we want to end on
            this.transform.position = new Vector2(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y));

            // make sure velocity is zeroed and update the animator
            body.velocity = Vector2.zero;

            // update the grid manager
            //gridManager.SetTileWalkable(new Vector2Int(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y)), 0);
            gridManager.HighlightWalkableTiles(this.transform.position, movesLeft);

            interacting = false;
            doneMoving = true;
        }
        
        // update the animator
        if(body.velocity == Vector2.zero)
        {
            animator.SetBool("Walking", false);
        } else
        {
            animator.SetBool("Walking", true);
            animator.SetFloat("X", body.velocity.normalized.x);
            animator.SetFloat("Y", body.velocity.normalized.y);
        }

    }

    public void StartTurn()
    {
        movesLeft = walkingRange;

        doneMoving = false;
    }

    public void UpdateMovesLeft(int spacesWalked)
    {
        if(spacesWalked >= movesLeft)
        {
            movesLeft = 0;
        }
        else
        {
            movesLeft -= spacesWalked;
        }

        //update the ui element
        turnManager.UpdateMoveCounter(movesLeft);
    }

    public bool Attack(CharacterPathfinding enemy)
    {
        // a return value of true means that we killed the enemy we were targeting
        return enemy.TakeDamage(this.attackDamage);
    }

    // can be used to damage or heal a character, a return value of true means the character is dead
    public bool TakeDamage(int incomingDamage)
    {
        this.health = Mathf.Clamp(this.health - incomingDamage, 0, maxHealth);

        return (this.health == 0);
    }
}
