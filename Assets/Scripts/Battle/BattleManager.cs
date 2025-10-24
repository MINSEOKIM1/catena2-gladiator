using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

using Action;
using Bucket.Manager;
using TMPro;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Battle
{
    public enum BattleState
    {
        Idle = 0,
        GladiatorAttack,
        EnemyAttack,
        End
    }

    public enum CommandType
    {
        Up = 0,
        Down = 1,
        Left = 2,
        Right = 3
    }

    public class BattleManager : MonoBehaviour
    {
        private const int CounterCommandCount = 5;
        
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
        public int gladiatorMaxHealth;
        public int enemyMaxHealth;
        
        [Space(10)]
        public CommandType[] currentCommands;
        public int currentCommandIndex;

        [Header("Cooldown")]
        public float attackCooldown;

        [Header("Sequences")]
        public ActionData[] gladiatorAttackSequence;
        public ActionData[] enemyAttackSequence;

        [Header("References")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private Animator gladiatorAnimator;
        [SerializeField] private Animator enemyAnimator;

        public TMP_Text casterText;
        private Coroutine _caseterCoroutine;

        private void Awake()
        {
            var attackAction = InputSystem.actions.FindAction("Attack");
            var dodgeAction = InputSystem.actions.FindAction("Dodge");
            var commandUpAction = InputSystem.actions.FindAction("CommandUp");
            var commandDownAction = InputSystem.actions.FindAction("CommandDown");
            var commandLeftAction = InputSystem.actions.FindAction("CommandLeft");
            var commandRightAction = InputSystem.actions.FindAction("CommandRight");

            attackAction.started += OnAttack;
            dodgeAction.started += OnDodge;
            commandUpAction.started += _ => OnCounter(CommandType.Up);
            commandDownAction.started += _ => OnCounter(CommandType.Down);
            commandLeftAction.started += _ => OnCounter(CommandType.Left);
            commandRightAction.started += _ => OnCounter(CommandType.Right);
        }

        private void Start()
        {
            battleState = BattleState.Idle;
            attackCooldownRemaining = 0f;
            isEnemyAttackAvailable = false;

            CasterSay("경기 시작합니다!");
            
            // TODO: Load sequences from other scene
            
            // Hard-coded, temporary values
            // TODO: Set health values
            gladiatorHealth = gladiatorMaxHealth = 100;
            enemyHealth = enemyMaxHealth = 100;
            
            uiManager.SetGladiatorHealth(1f);
            uiManager.SetEnemyHealth(1f);
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
                            uiManager.SetCounterTimingBar(1 - actionRatio);
                        }
                        else
                        {
                            EnemyAttackTimeout();
                        }

                        actionTimer += Time.deltaTime;
                    }

                    break;
                case BattleState.End:
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(battleState), battleState, null);
            }

            attackCooldownRemaining -= Time.deltaTime;
        }

        private IEnumerator CasterText()
        {
            casterText.gameObject.SetActive(true);

            yield return new WaitForSeconds(2f);
            
            casterText.gameObject.SetActive(false);
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
                        EnemyAttackTimeout();
                    }
                        
                    WrongAction();
                    break;
                    
                default:
                    WrongAction();
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
            
            gladiatorAnimator.SetBool("PreAttack", true);
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
                gladiatorAnimator.SetTrigger("Attack");
                enemyAnimator.SetTrigger("Hit");
                
                string tmp;
                switch (combo)
                {
                    case 0:
                        tmp = "첫 번째 공격이 성공합니다! 연계를 이어나갑니다!";
                        break;
                    case 1:
                        tmp = "두 번째 공격도 성공합니다! 마지막 공격을 준비합니다!";
                        DataManager.Instance.popularity += 1;
                        break;
                    case 2:
                        tmp = "마지막 일격 성공합니다! 관객들이 환호합니다!";
                        DataManager.Instance.popularity += 3;
                        break;
                    default:
                        tmp = "에러임";
                        break;
                }
                CasterSay(tmp);
                
                Debug.Log("Hit");

                var damageMin = currentAction.damageMin;
                var damageMax = currentAction.damageMax;
                var damage = Random.Range(damageMin, damageMax + 1);    // Min/Max inclusive
                DamageEnemy(damage);
                
                var delay = currentAction.delayAfterAction;
                
                combo++;
                
                if (combo == gladiatorAttackSequence.Length || enemyHealth <= 0)
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

                string tmp = "공격에 실패합니다... 관객들이 야유를 하는 모습입니다.";
                DataManager.Instance.popularity -= 2;
                CasterSay(tmp);

                FinishAttack();
            }
        }

        private void CasterSay(string text)
        {
            if (_caseterCoroutine != null) StopCoroutine(_caseterCoroutine);
            _caseterCoroutine = StartCoroutine(CasterText()); 
            casterText.text = text;
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

            gladiatorAnimator.SetBool("PreAttack", false);
        }

        private void OnCounter(CommandType type)
        {
            if (battleState != BattleState.EnemyAttack || !isActioning)
            {
                WrongAction();
                return;
            }
            
            PerformCommand(type);
        }

        private void PerformCommand(CommandType type)
        {
            Debug.Log($"Perform gladiator command, index: {currentCommandIndex}, type: {type}");

            if (type == currentCommands[currentCommandIndex])
            {
                uiManager.SetCounterCommandActive(currentCommandIndex);
                
                currentCommandIndex++;
                if (currentCommandIndex >= CounterCommandCount)
                {
                    PerformCounter();
                }
            }
            else
            {
                EnemyAttackTimeout();
            }
        }

        private void PerformCounter()
        {
            Debug.Log($"Perform gladiator counter");
            
            gladiatorAnimator.SetTrigger("Counter");
            enemyAnimator.SetTrigger("Attack");
            enemyAnimator.SetBool("PreAttack", false);
            
            string tmp = "막아냅니다!";
            CasterSay(tmp);
            
            
            if (combo == enemyAttackSequence.Length - 1 || gladiatorHealth <= 0)
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
            
            gladiatorAnimator.SetTrigger("Dodge");
            enemyAnimator.SetTrigger("Attack");
            enemyAnimator.SetBool("PreAttack", false);
            
            string tmp = "얍삽하게 회피하는 모습입니다... 관객들이 야유를 하는 모습입니다.";
            DataManager.Instance.popularity -= 5;
            CasterSay(tmp);
            
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
            
            enemyAnimator.SetBool("PreAttack", true);
            
            battleState = BattleState.EnemyAttack;
            isEnemyAttackAvailable = false;
            isActioning = true;
            combo = 0;
            
            actionTimer = 0f;
            actionDuration = enemyAttackSequence[combo].durationBase;
            
            SetCurrentCounterCommands(CounterCommandCount);
            uiManager.SetCounterCommands(currentCommands);
            uiManager.ToggleCounterPopup(true);
        }

        private void SetCurrentCounterCommands(int count)
        {
            currentCommands = new CommandType[count];
            for (var i = 0; i < count; i++)
            {
                var command = (CommandType)Random.Range(0, 4);
                currentCommands[i] = command;
            }
            currentCommandIndex = 0;
        }

        private IEnumerator ContinueEnemyAttackAfterDelay(float delay)
        {
            isActioning = false;
            combo++;
            uiManager.ToggleCounterPopup(false);
            uiManager.ClearCounterCommands();
            
            yield return new WaitForSeconds(delay);

            isActioning = true;
            actionTimer = 0f;
            actionDuration = enemyAttackSequence[combo].durationBase;
            
            enemyAnimator.SetBool("PreAttack", true);
            
            SetCurrentCounterCommands(CounterCommandCount);
            uiManager.SetCounterCommands(currentCommands);
            uiManager.ToggleCounterPopup(true);
        }

        private void EnemyAttackTimeout()
        {
            Debug.Log("Enemy attack timeout");

            var currentAction = enemyAttackSequence[combo];

            var damageMin = currentAction.damageMin;
            var damageMax = currentAction.damageMax;
            var damage = Random.Range(damageMin, damageMax + 1);    // Min/Max inclusive
            DamageGladiator(damage);
            
            gladiatorAnimator.SetTrigger("Hit");
            enemyAnimator.SetTrigger("Attack");
            enemyAnimator.SetBool("PreAttack", false);
            
            string tmp = "세게 들어갑니다! 아프겠당";
            CasterSay(tmp);


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
            
            uiManager.ClearCounterCommands();
            uiManager.ToggleCounterPopup(false);
        }

        private void DamageGladiator(int damage)
        {
            Debug.Log($"Gladiator takes {damage} damage");

            gladiatorHealth -= damage;
            if (gladiatorHealth < 0)
            {
                string tmp = "패배했습니다...";
                CasterSay(tmp);
                DataManager.Instance.hp = 1;
                
                StartCoroutine(GoToScene());
                
                gladiatorHealth = 0;

                battleState = BattleState.End;
            }

            var healthRatio = (float)gladiatorHealth / gladiatorMaxHealth;
            uiManager.SetGladiatorHealth(healthRatio);
            
            
            // TODO: Implement game over
            
            
        }

        private void DamageEnemy(int damage)
        {
            Debug.Log($"Enemy takes {damage} damage");

            enemyHealth -= damage;
            if (enemyHealth < 0)
            {
                string tmp = "통쾌한 승리! 끼얏호우";
                DataManager.Instance.popularity += 3;
                DataManager.Instance.money += 15;
                DataManager.Instance.hp = gladiatorHealth;
                CasterSay(tmp);

                StartCoroutine(GoToScene());
                
                enemyHealth = 0;
                
                battleState = BattleState.End;
            }
            
            var healthRatio = (float)enemyHealth / enemyMaxHealth;
            uiManager.SetEnemyHealth(healthRatio);
            
            
            // TODO: Implement enemy defeat
            
            
        }


        private IEnumerator GoToScene()
        {
            yield return new WaitForSeconds(2f);
            DataManager.Instance.time += 2;
            SceneManager.LoadScene("Scenes/Map");
        }
    }
}
