using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterIdle : CharacterState
{
    CharacterState walkingState;
    CharacterState waitingState;
    private bool tilesHighlighted;

    private void Start()
    {
        walkingState = this.gameObject.GetComponentInChildren<PlayerCharacterWalking>();
        waitingState = this.gameObject.GetComponentInChildren<PlayerCharacterWaiting>();
        tilesHighlighted = false;
    }

    public override CharacterState TickState(float delta)
    {
        //Debug.Log("Current State = Idle");

        if (!characterManager.turn) { tilesHighlighted = false; return waitingState; }

        if (!tilesHighlighted)
        {
            characterManager.charPathfinding.gridManager.HighlightWalkableTiles(this.transform.position, characterManager.charPathfinding.movesLeft);
            tilesHighlighted = true;
        }
        

        // get the mouse position on the grid
        var pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var noZ = new Vector3(pos.x, pos.y);
        Vector3Int mouseCell = FindObjectOfType<Grid>().WorldToCell(noZ);

        // update the attack indicator
        if (characterManager.charPathfinding.gridManager.CheckIfPointOnGrid(mouseCell))
        {
            CharacterManager enemy = characterManager.charPathfinding.gridManager.GetCharacterOnTile(new Vector2Int(Mathf.RoundToInt(noZ.x), Mathf.RoundToInt(noZ.y)), characterManager);
            if (enemy != null && enemy.currentState != enemy.deadState && characterManager.charPathfinding.movesLeft >= characterManager.characterStats.AttackCost && (Mathf.RoundToInt(Mathf.Abs(this.transform.position.x - noZ.x)) + Mathf.RoundToInt(Mathf.Abs(this.transform.position.y - noZ.y))) <= characterManager.characterStats.AttackRange)
            {
                if (!characterManager.attackIndicator.gameObject.activeInHierarchy || !characterManager.attackIndicator.targetText.text.Contains(enemy.gameObject.name))
                {
                    characterManager.attackIndicator.UpdateTargetText(enemy.gameObject.name);
                    characterManager.attackIndicator.UpdateCostText(characterManager.characterStats.AttackCost.ToString());
                    characterManager.attackIndicator.gameObject.SetActive(true);
                }
            }
            else
            {
                // disable the attack indicator
                if (characterManager.attackIndicator.gameObject.activeInHierarchy)
                {
                    characterManager.attackIndicator.UpdateTargetText("");
                    characterManager.attackIndicator.UpdateCostText("");
                    characterManager.attackIndicator.gameObject.SetActive(false);
                }
                
            }
        }

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
                        // check to see if character has enough action points left to attack
                        if (characterManager.charPathfinding.movesLeft < characterManager.characterStats.AttackCost)
                        {
                            Debug.Log("Not enough action points left to attack");
                            return this;
                        }
                        else
                        {
                            characterManager.charPathfinding.UpdateMovesLeft(characterManager.characterStats.AttackCost);
                            tilesHighlighted = false;
                        }

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
                    tilesHighlighted = false;
                    return walkingState;
                }
            }
        }

        return this;
    }
}
