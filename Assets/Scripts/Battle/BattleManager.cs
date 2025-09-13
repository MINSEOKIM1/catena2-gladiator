using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

using Action;

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
        [Header("Values")]
        public BattleState battleState;
        
        [Space(10)]
        public float attackCooldownRemaining;

        [Space(10)]
        public float actionTimer;
        public float actionDuration;
        public bool isActioning;
        public int combo;

        [Header("Cooldown")]
        public float attackCooldown;

        [Header("Sequences")]
        public ActionData[] gladiatorAttackSequence;
        public ActionData[] enemyAttackSequence;

        [Header("References")]
        [SerializeField] private UIManager uiManager;
        
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
                    if (isActioning)
                    {
                        if (actionTimer < actionDuration)
                        {
                            var actionRatio = actionTimer / actionDuration;
                            uiManager.UpdateAttackTimingBar(actionRatio);
                        }
                        else
                        {
                            FinishAttack();
                        }

                        actionTimer += Time.deltaTime;
                    }

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
            actionTimer = 0f;
            actionDuration = gladiatorAttackSequence[0].durationBase;
            isActioning = true;
            combo = 0;

            _attackAction.started -= StartAttack;
            _attackAction.started += PerformAttack;

            ToggleAttackUI(true);
            // TODO: Attacking animation
        }

        private void PerformAttack(InputAction.CallbackContext _)
        {
            if (battleState != BattleState.GladiatorAttack ||
                !isActioning) return;
            
            Debug.Log("Perform gladiator attack");
            
            var correctTimingStart = gladiatorAttackSequence[combo].correctTimingStartRatio;
            var correctTimingEnd = gladiatorAttackSequence[combo].correctTimingEndRatio;
            if (actionTimer >= correctTimingStart && actionTimer <= correctTimingEnd)
            {
                Debug.Log("Hit");

                isActioning = false;
                var delay = gladiatorAttackSequence[combo].delayAfterAction;
                combo++;

                // TODO: Deal damage logic

                if (combo == gladiatorAttackSequence.Length)
                {
                    FinishAttack();
                }
                else
                {
                    StartCoroutine(ContinueAttackAfterDelay(delay));
                }
            }
            else
            {
                Debug.Log("Miss");

                FinishAttack();
            }
        }

        private IEnumerator ContinueAttackAfterDelay(float delay)
        {
            ToggleAttackUI(false);
            
            yield return new WaitForSeconds(delay);

            actionTimer = 0f;
            actionDuration = gladiatorAttackSequence[combo].durationBase;
            isActioning = true;

            ToggleAttackUI(true);
        }

        private void ToggleAttackUI(bool value)
        {
            if (value)
            {
                var startRatio = gladiatorAttackSequence[combo].correctTimingStartRatio;
                var endRatio = gladiatorAttackSequence[combo].correctTimingEndRatio;

                uiManager.SetAttackCorrectTimingRange(startRatio, endRatio);
                uiManager.UpdateAttackTimingBar(0f);
            }

            uiManager.ToggleAttackTimingBar(value);
        }

        private void FinishAttack()
        {
            if (battleState != BattleState.GladiatorAttack) return;
            
            Debug.Log("Finish gladiator attack");
            
            ToggleAttackUI(false);

            battleState = BattleState.Idle;
            attackCooldownRemaining = attackCooldown;

            _attackAction.started -= PerformAttack;
            _attackAction.started += StartAttack;

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
