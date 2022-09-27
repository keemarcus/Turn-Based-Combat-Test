using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterIdle : CharacterState
{
    CharacterState walkingState;
    CharacterState waitingState;

    private void Start()
    {
        walkingState = this.gameObject.GetComponentInChildren<PlayerCharacterWalking>();
        waitingState = this.gameObject.GetComponentInChildren<PlayerCharacterWaiting>();
    }

    public override CharacterState TickState(float delta)
    {
        //Debug.Log("Current State = Idle");

        if (!characterManager.turn) { return waitingState; }

        characterManager.charPathfinding.gridManager.HighlightWalkableTiles(this.transform.position, characterManager.charPathfinding.movesLeft);

        // get the mouse position on the grid
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var noZ = new Vector3(pos.x, pos.y);
        Vector3Int mouseCell = FindObjectOfType<Grid>().WorldToCell(noZ);

        // only register clicks if we're not already moving
        if (Input.GetMouseButtonUp(1) && characterManager.body.velocity == Vector2.zero)
        {
            if (characterManager.charPathfinding.gridManager.CheckIfPointOnGrid(mouseCell))
            {
                if (characterManager.charPathfinding.gridManager.CheckIfCharacterOnTile(new Vector2Int(Mathf.RoundToInt(noZ.x), Mathf.RoundToInt(noZ.y))) && (Mathf.RoundToInt(Mathf.Abs(this.transform.position.x - noZ.x)) + Mathf.RoundToInt(Mathf.Abs(this.transform.position.y - noZ.y))) <= characterManager.characterStats.AttackRange)
                {
                    CharacterManager enemy = characterManager.charPathfinding.gridManager.GetCharacterOnTile(new Vector2Int(Mathf.RoundToInt(noZ.x), Mathf.RoundToInt(noZ.y)), characterManager);
                    if (enemy == null || enemy.currentState == enemy.deadState)
                    {
                        //Debug.Log("No character found");
                    }
                    else
                    {
                        if (characterManager.Attack(enemy))
                        {
                            Debug.Log("Killed " + enemy.gameObject.name);
                        }
                        else
                        {
                            Debug.Log(enemy.gameObject.name + " took " + (this.characterManager.characterStats.BaseDamage + this.characterManager.characterStats.HitBonus - enemy.characterStats.DamageResistance) + " damage");
                        }
                    }

                }
                else if (characterManager.charPathfinding.gridManager.CheckIfPointInRange(characterManager.charPathfinding.movesLeft, this.transform.position, noZ))
                {
                    characterManager.charPathfinding.MoveToTile(mouseCell.x + 1, mouseCell.y + 1);
                    return walkingState;
                }
            }
        }

        return this;
    }
}
