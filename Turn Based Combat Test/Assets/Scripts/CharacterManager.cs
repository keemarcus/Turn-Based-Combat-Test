using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour
{
    CharacterState currentState;
    public CharacterPathfinding charPathfinding;
    
    public Rigidbody2D body;
    Animator animator;

    public bool turn;
    public bool doneMoving;
    public bool interacting;
    public int attackRange = 1;
    public int maxHealth = 50;
    public int health;
    public int attackDamage = 10;

    public TurnManager turnManager;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        charPathfinding = GetComponent<CharacterPathfinding>();
        charPathfinding.gridManager = FindObjectOfType<GridManager>();
        turnManager = FindObjectOfType<Camera>().GetComponentInChildren<TurnManager>();

        health = maxHealth;

        interacting = false;
        doneMoving = false;
        turn = false;

        // set the initial characterState to waiting (not their turn)
        currentState = this.gameObject.GetComponentInChildren<PlayerCharacterWaiting>();
    }

    private void Update()
    {
        currentState = currentState.TickState(Time.deltaTime);

        // update the animator
        if (body.velocity == Vector2.zero)
        {
            animator.SetBool("Walking", false);
        }
        else
        {
            animator.SetBool("Walking", true);
            animator.SetFloat("X", body.velocity.normalized.x);
            animator.SetFloat("Y", body.velocity.normalized.y);
        }
    }

    public void StartTurn()
    {
        charPathfinding.movesLeft = charPathfinding.walkingRange;

        doneMoving = false;
    }

    public bool Attack(CharacterManager enemy)
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
