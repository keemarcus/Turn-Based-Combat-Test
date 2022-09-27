using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CharacterPathfinding : MonoBehaviour
{
    [Header("Pathfinding References")]
    public GridManager gridManager;
    public List<PathFind.Point> path;
    //public int walkingRange = 5;
    public int movesLeft;
    PathFind.Point _from;
    PathFind.Point _to;

    CharacterManager characterManager;

    private void Awake()
    {
        characterManager = this.gameObject.GetComponentInChildren<CharacterManager>();
    }

    public void MoveToTile(int targetX, int targetY)
    {
        _from = new PathFind.Point(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y));
        _to = new PathFind.Point(targetX, targetY);
        path = gridManager.GetPath(_from, _to, characterManager.characterStats.ActionPoints);

        // update the grid
        gridManager.SetTileWalkable(new Vector2Int(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y)), 1f);
        gridManager.SetTileWalkable(new Vector2Int(targetX, targetY), -1f);

        characterManager.doneMoving = false;
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
        characterManager.turnManager.UpdateMoveCounter(movesLeft);
    }

    
}
