using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CharacterManager : MonoBehaviour
{
    public CharacterState currentState;
    public CharacterState deadState;
    public CharacterPathfinding charPathfinding;
    public AttackIndicatorManager attackIndicator;
    public HealthBarManager healthBarManager;
    
    public Rigidbody2D body;
    Animator animator;

    public bool turn;
    public bool doneMoving;
    public bool interacting;

    public CharacterStats characterStats;

    [Serializable]
    public struct CharacterStats
    {
        [Header("Character Stats")]
        public int MaxHP;
        public int CurrentHP;
        public int ActionPoints;
        public int AttackRange;
        public int AttackCost;
        public int BaseDamage;
        public int HitBonus;
        public int DamageResistance;

        public CharacterStats(int maxHP, int actionPoints, int attackRange, int attactCost, int baseDamage, int hitBonus, int damageResistance)
        {
            MaxHP = maxHP;
            CurrentHP = MaxHP;
            ActionPoints = actionPoints;
            AttackRange = attackRange;
            AttackCost = attactCost;
            BaseDamage = baseDamage;
            HitBonus = hitBonus;
            DamageResistance = damageResistance;
        }
    }

    public TurnManager turnManager;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        charPathfinding = GetComponent<CharacterPathfinding>();
        charPathfinding.gridManager = FindObjectOfType<GridManager>();
        turnManager = FindObjectOfType<Camera>().GetComponentInChildren<TurnManager>();

        attackIndicator = FindObjectOfType<AttackIndicatorManager>();
        healthBarManager = this.gameObject.GetComponentInChildren<HealthBarManager>();

        characterStats.CurrentHP = characterStats.MaxHP;
        this.TakeDamage(0);

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
        charPathfinding.movesLeft = characterStats.ActionPoints;

        doneMoving = false;
    }

    public bool Attack(CharacterManager enemy)
    {
        // calculate the damage amount
        int damage = this.characterStats.BaseDamage + this.characterStats.HitBonus - enemy.characterStats.DamageResistance;

        // a return value of true means that we killed the enemy we were targeting
        return enemy.TakeDamage(damage);
    }

    // can be used to damage or heal a character, a return value of true means the character is dead
    public bool TakeDamage(int incomingDamage)
    {
        this.characterStats.CurrentHP = Mathf.Clamp(this.characterStats.CurrentHP - incomingDamage, 0, this.characterStats.MaxHP);

        // update the healthbar
        this.healthBarManager.SetValue((float)this.characterStats.CurrentHP / (float)this.characterStats.MaxHP);

        if(this.characterStats.CurrentHP == 0)
        {
            currentState = deadState;
        }
        return (this.characterStats.CurrentHP == 0);
    }
}
