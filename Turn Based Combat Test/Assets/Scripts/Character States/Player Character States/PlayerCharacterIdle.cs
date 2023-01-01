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

                //Debug.Log("door indicator code");
                
                //Debug.Log(Mathf.RoundToInt(Mathf.Abs(this.transform.position.x - noZ.x)) + Mathf.RoundToInt(Mathf.Abs(this.transform.position.y - noZ.y)));
                //update the door indicator
                DoorManager door = characterManager.charPathfinding.gridManager.GetDoorOnTile(new Vector2Int(Mathf.RoundToInt(noZ.x), Mathf.RoundToInt(noZ.y)));
                //Debug.Log("Condition 1: " + (door != null));
                //Debug.Log("Condition 2: " + (characterManager.charPathfinding.movesLeft >= characterManager.characterStats.OpenCost));
                //Debug.Log("Condition 3: " + (Mathf.RoundToInt(Mathf.Abs(this.transform.position.x - noZ.x)) + Mathf.RoundToInt(Mathf.Abs(this.transform.position.y - noZ.y)) <= 1f));
                if (door != null && characterManager.charPathfinding.movesLeft >= characterManager.characterStats.OpenCost && (Mathf.RoundToInt(Mathf.Abs(this.transform.position.x - noZ.x)) + Mathf.RoundToInt(Mathf.Abs(this.transform.position.y - noZ.y))) <= 1f)
                {
                    if (!characterManager.doorIndicator.gameObject.activeInHierarchy)
                    {
                        characterManager.doorIndicator.UpdateCostText(characterManager.characterStats.OpenCost.ToString());
                        characterManager.doorIndicator.gameObject.SetActive(true);
                    }
                }
                else
                {
                    // disable the door indicator
                    if (characterManager.doorIndicator.gameObject.activeInHierarchy)
                    {
                        characterManager.doorIndicator.UpdateCostText("");
                        characterManager.doorIndicator.gameObject.SetActive(false);
                    }
                }
            }
        }

        // only register clicks if we're not already moving
        if (Input.GetMouseButtonUp(1) && characterManager.body.velocity == Vector2.zero)
        {
            if (characterManager.charPathfinding.gridManager.CheckIfPointOnGrid(mouseCell))
            {
                // handle attacks
                if(characterManager.attackIndicator.isActiveAndEnabled)
                //if (characterManager.charPathfinding.gridManager.CheckIfCharacterOnTile(new Vector2Int(Mathf.RoundToInt(noZ.x), Mathf.RoundToInt(noZ.y))) && (Mathf.RoundToInt(Mathf.Abs(this.transform.position.x - noZ.x)) + Mathf.RoundToInt(Mathf.Abs(this.transform.position.y - noZ.y))) <= characterManager.characterStats.AttackRange)
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
                // handle door interactions
                if (characterManager.doorIndicator.isActiveAndEnabled)
                {
                    DoorManager door = characterManager.charPathfinding.gridManager.GetDoorOnTile(new Vector2Int(Mathf.RoundToInt(noZ.x), Mathf.RoundToInt(noZ.y)));
                    if (door == null)
                    {
                        //Debug.Log("No door found");
                    }
                    else
                    {
                        // check to see if character has enough action points left to open the door
                        if (characterManager.charPathfinding.movesLeft < characterManager.characterStats.OpenCost)
                        {
                            Debug.Log("Not enough action points left to open");
                            return this;
                        }
                        else
                        {
                            characterManager.charPathfinding.UpdateMovesLeft(characterManager.characterStats.OpenCost);
                            tilesHighlighted = false;
                            door.ToggleState();
                        }
                    }
                }
                // handle movement
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
