using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacterDead : CharacterState
{
    CharacterState waitingState;
    CharacterState idleState;

    private void Start()
    {
        idleState = this.gameObject.GetComponentInChildren<PlayerCharacterIdle>();
        waitingState = this.gameObject.GetComponentInChildren<PlayerCharacterWaiting>();
    }
    public override CharacterState TickState(float delta)
    {
        // if the character has regained some hp, put them back in the idle state, otherwise just return dead state
        if(characterManager.characterStats.CurrentHP > 0)
        {
            if (characterManager.turn)
            {
                return idleState;
            }
            else
            {
                return waitingState;
            }
        }
        else
        {
            return this;
        }
    }
}
