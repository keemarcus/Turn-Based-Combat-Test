using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterWaiting : CharacterState
{
    CharacterState idleState;

    private void Start()
    {
        idleState = this.gameObject.GetComponentInChildren<PlayerCharacterIdle>();
    }
    public override CharacterState TickState(float delta)
    {
        //Debug.Log("Current State = Waiting");

        if (characterManager.turn)
        {
            return idleState;
        }
        else
        {
            return this;
        }
    }
}
