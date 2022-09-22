using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterWalking : CharacterState
{
    CharacterState idleState;

    private void Start()
    {
        idleState = this.gameObject.GetComponentInChildren<PlayerCharacterIdle>();
    }

    public override CharacterState TickState(float delta)
    {
        //Debug.Log("Current State = Walking");

        // move through the path if we have one
        if (characterManager.charPathfinding.path != null && characterManager.charPathfinding.path.Count > 0)
        {
            characterManager.interacting = true;

            if (Vector2.Distance(this.transform.position, new Vector2(characterManager.charPathfinding.path[0].x, characterManager.charPathfinding.path[0].y)) >= .1f)
            {
                characterManager.body.velocity = new Vector2(characterManager.charPathfinding.path[0].x - this.transform.position.x, characterManager.charPathfinding.path[0].y - this.transform.position.y).normalized * 3f;
            }
            else
            {
                characterManager.charPathfinding.path.RemoveAt(0);
                // remove a space from our moves left
                characterManager.charPathfinding.UpdateMovesLeft(1);
            }

            return this;
        }
        else if (!characterManager.doneMoving)// we're at the end of the path
        {
            // make sure we're exactly on the space we want to end on
            this.transform.position = new Vector2(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y));

            // make sure velocity is zeroed and update the animator
            characterManager.body.velocity = Vector2.zero;

            // update the grid manager
            //gridManager.SetTileWalkable(new Vector2Int(Mathf.RoundToInt(this.transform.position.x), Mathf.RoundToInt(this.transform.position.y)), 0);
            

            characterManager.interacting = false;
            characterManager.doneMoving = true;

            return idleState;
        }

        return this;
    }
}
