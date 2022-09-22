using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterState : MonoBehaviour
{
    protected CharacterManager characterManager;
    private void Awake()
    {
        this.characterManager = this.gameObject.GetComponentInChildren<CharacterManager>();
    }
    public abstract CharacterState TickState(float delta);

}
