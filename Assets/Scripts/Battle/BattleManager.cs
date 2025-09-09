using System;
using UnityEngine;
using UnityEngine.InputSystem;

public enum BattleState
{
    Idle = 0,
    GladiatorAttack,
    EnemyAttack
}

namespace Battle
{
    public class BattleManager : MonoBehaviour
    {
        [Header("Configurations")]
        public float defaultStateTime;
        public float attackCooldown;

        [Header("Status")]
        public BattleState battleState;
        public float attackCooldownRemaining;

        private InputAction _attackAction;

        private void Awake()
        {
            _attackAction = InputSystem.actions.FindAction("Attack");
        }

        private void Start()
        {
            // TODO: Load sequences from other scene

            _attackAction.started += StartAttack;

            battleState = BattleState.Idle;
            attackCooldownRemaining = 0f;
        }

        private void Update()
        {
            switch (battleState)
            {
                case BattleState.Idle:
                    var cooldownRemainingRatio = attackCooldownRemaining / attackCooldown;
                    if (cooldownRemainingRatio is > 0.4f and < 0.5f)
                    {
                        StartEnemyAttack();
                    }

                    break;
                
                case BattleState.GladiatorAttack:
                    // TODO: Handle gladiator attack sequence
                    FinishAttack(); // Temp
                    
                    break;
                
                case BattleState.EnemyAttack:
                    // TODO
                    FinishEnemyAttack(); // Temp
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException(nameof(battleState), battleState, null);
            }

            attackCooldownRemaining -= Time.deltaTime;
        }

        private void StartAttack(InputAction.CallbackContext _)
        {
            if (battleState != BattleState.Idle ||
                attackCooldownRemaining > 0f) return;
            
            Debug.Log("Start gladiator attack");

            battleState = BattleState.GladiatorAttack;
            
            // TODO: Perform attack
        }

        private void FinishAttack()
        {
            if (battleState != BattleState.GladiatorAttack) return;
            
            Debug.Log("Finish gladiator attack");

            battleState = BattleState.Idle;
            attackCooldownRemaining = attackCooldown;
            
            // TODO: Finish attack
        }

        private void StartEnemyAttack()
        {
            if (battleState != BattleState.Idle) return;
            
            Debug.Log("Start enemy attack");

            battleState = BattleState.EnemyAttack;
            
            // TODO: Perform enemy attack
        }

        private void FinishEnemyAttack()
        {
            if (battleState != BattleState.EnemyAttack) return;
            
            Debug.Log("Finish enemy attack");

            battleState = BattleState.Idle;
            
            // TODO: Finish enemy attack
        }
    }
}
