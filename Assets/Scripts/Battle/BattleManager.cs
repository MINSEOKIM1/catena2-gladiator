using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

using Action;
using Random = UnityEngine.Random;

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
        public bool isEnemyAttackAvailable;

        [Space(10)]
        public float actionTimer;
        public float actionDuration;
        public bool isActioning;
        public int combo;

        [Space(10)]
        public int gladiatorHealth;
        public int enemyHealth;

        [Header("Cooldown")]
        public float attackCooldown;

        [Header("Sequences")]
        public ActionData[] gladiatorAttackSequence;
        public ActionData[] enemyAttackSequence;

        [Header("References")]
        [SerializeField] private UIManager uiManager;

        private void Awake()
        {
            var attackAction = InputSystem.actions.FindAction("Attack");
            var dodgeAction = InputSystem.actions.FindAction("Dodge");

            attackAction.started += OnAttack;
            dodgeAction.started += OnDodge;
        }

        private void Start()
        {
            battleState = BattleState.Idle;
            attackCooldownRemaining = 0f;
            isEnemyAttackAvailable = false;
            
            // TODO: Load sequences from other scene
            
            // Hard-coded, temporary values
            // TODO: Set health values
            gladiatorHealth = 100;
            enemyHealth = 100;
        }

        private void Update()
        {
            switch (battleState)
            {
                case BattleState.Idle:
                    // Hard-coded, temporary behavior
                    // TODO: replace this later
                    var cooldownRemainingRatio = attackCooldownRemaining / attackCooldown;
                    if (isEnemyAttackAvailable && cooldownRemainingRatio  < 0.5f)
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
                            uiManager.SetAttackTimingBar(actionRatio);
                        }
                        else
                        {
                            FinishAttack();
                        }

                        actionTimer += Time.deltaTime;
                    }

                    break;

                case BattleState.EnemyAttack:
                    if (isActioning)
                    {
                        if (actionTimer < actionDuration)
                        {
                            var actionRatio = actionTimer / actionDuration;
                            uiManager.SetCounterPopupIconFill(1 - actionRatio);
                        }
                        else
                        {
                            EnemyAttackTimeout();
                        }

                        actionTimer += Time.deltaTime;
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(battleState), battleState, null);
            }

            attackCooldownRemaining -= Time.deltaTime;
        }

        private void OnAttack(InputAction.CallbackContext _)
        {
            switch (battleState)
            {
                case BattleState.Idle:
                    if (attackCooldownRemaining <= 0f)
                    {
                        StartAttack();
                    }
                    else
                    {
                        WrongAction();
                    }
                    break;
                
                case BattleState.GladiatorAttack:
                    if (isActioning)
                    {
                        PerformAttack();
                    }
                    else
                    {
                        WrongAction();
                    }
                    break;
                
                case BattleState.EnemyAttack:
                    if (isActioning)
                    {
                        PerformCounter();
                    }
                    else
                    {
                        WrongAction();
                    }
                    break;
            }
        }

        public void OnDodge(InputAction.CallbackContext _)
        {
            var dodgeAvailable = battleState == BattleState.EnemyAttack && isActioning;

            if (dodgeAvailable)
            {
                PerformDodge();
            }
            else
            {
                WrongAction();
            }
        }

        private void WrongAction()
        {
            Debug.Log("Wrong action");
            
            // TODO: Implement this
        }

        private void StartAttack()
        {
            Debug.Log("Start gladiator attack");

            battleState = BattleState.GladiatorAttack;
            isActioning = true;
            combo = 0;

            actionTimer = 0f;
            actionDuration = gladiatorAttackSequence[0].durationBase;

            ToggleAttackUI(true);
            // TODO: Attacking animation
        }

        private void PerformAttack()
        {
            Debug.Log("Perform gladiator attack");

            var currentAction = gladiatorAttackSequence[combo];
            
            var actionRatio = actionTimer / actionDuration;
            var correctTimingStart = currentAction.correctTimingStartRatio;
            var correctTimingEnd = currentAction.correctTimingEndRatio;
            if (actionRatio >= correctTimingStart && actionRatio <= correctTimingEnd)
            {
                Debug.Log("Hit");

                var damageMin = currentAction.damageMin;
                var damageMax = currentAction.damageMax;
                var damage = Random.Range(damageMin, damageMax + 1);    // Min/Max inclusive
                DamageEnemy(damage);
                
                var delay = currentAction.delayAfterAction;
                
                combo++;
                
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
            isActioning = false;
            ToggleAttackUI(false);
            
            yield return new WaitForSeconds(delay);

            actionTimer = 0f;
            actionDuration = gladiatorAttackSequence[combo].durationBase;
            isActioning = true;

            ToggleAttackUI(true);
        }

        private void FinishAttack()
        {
            Debug.Log("Finish gladiator attack");

            battleState = BattleState.Idle;
            isActioning = false;
            attackCooldownRemaining = attackCooldown;
            isEnemyAttackAvailable = true;
            
            ToggleAttackUI(false);
        }

        private void PerformCounter()
        {
            Debug.Log("Perform gladiator counter");

            if (combo == enemyAttackSequence.Length - 1)
            {
                FinishEnemyAttack();
            }
            else
            {
                var delay = enemyAttackSequence[combo].delayAfterAction;
                StartCoroutine(ContinueEnemyAttackAfterDelay(delay));
            }
        }

        private void PerformDodge()
        {
            Debug.Log("Dodge enemy attack");
            
            FinishEnemyAttack();
        }

        private void ToggleAttackUI(bool value)
        {
            if (value)
            {
                var startRatio = gladiatorAttackSequence[combo].correctTimingStartRatio;
                var endRatio = gladiatorAttackSequence[combo].correctTimingEndRatio;

                uiManager.SetAttackCorrectTimingRange(startRatio, endRatio);
                uiManager.SetAttackTimingBar(0f);
            }

            uiManager.ToggleAttackTimingBar(value);
        }

        private void StartEnemyAttack()
        {
            Debug.Log("Start enemy attack");
            
            battleState = BattleState.EnemyAttack;
            isEnemyAttackAvailable = false;
            isActioning = true;
            combo = 0;
            
            actionTimer = 0f;
            actionDuration = enemyAttackSequence[combo].durationBase;
            uiManager.ToggleCounterPopup(true);
        }

        private IEnumerator ContinueEnemyAttackAfterDelay(float delay)
        {
            isActioning = false;
            combo++;
            uiManager.ToggleCounterPopup(false);
            
            yield return new WaitForSeconds(delay);

            isActioning = true;
            actionTimer = 0f;
            actionDuration = enemyAttackSequence[combo].durationBase;
            
            uiManager.ToggleCounterPopup(true);

            // TODO: Implement this
        }

        private void EnemyAttackTimeout()
        {
            Debug.Log("Enemy attack timeout");

            var currentAction = enemyAttackSequence[combo];

            var damageMin = currentAction.damageMin;
            var damageMax = currentAction.damageMax;
            var damage = Random.Range(damageMin, damageMax + 1);    // Min/Max inclusive
            DamageGladiator(damage);

            if (combo == enemyAttackSequence.Length - 1)
            {
                FinishEnemyAttack();
            }
            else
            {
                var delay = currentAction.delayAfterAction;

                StartCoroutine(ContinueEnemyAttackAfterDelay(delay));
            }
        }

        private void FinishEnemyAttack()
        {
            Debug.Log("Finish enemy attack");

            battleState = BattleState.Idle;

            // TODO: Finish enemy attack
            
            uiManager.ToggleCounterPopup(false);
        }

        private void DamageGladiator(int damage)
        {
            Debug.Log($"Gladiator takes {damage} damage");

            gladiatorHealth -= damage;
            if (gladiatorHealth < 0) gladiatorHealth = 0;
            
            // TODO: Implement UI update
            
            // TODO: Implement game over
        }

        private void DamageEnemy(int damage)
        {
            Debug.Log($"Enemy takes {damage} damage");

            enemyHealth -= damage;
            if (enemyHealth < 0) enemyHealth = 0;
            
            // TODO: Implement UI update
            
            // TODO: Implement enemy defeat
        }
    }
}
