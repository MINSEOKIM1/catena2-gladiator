using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BattleManager : MonoBehaviour
{
    private bool _isAttacking = false;
    private bool _isCountering = false;

    private bool _isAttackAvailable = false;

    private float _attackCooldownElapsed;
    [SerializeField] private float attackCooldown;
    
    private float _timingBarElapsed = 0f;
    private float _timingBar = 1f;
    private float _goodTiming = 0.5f;
    private float _goodTimingRange = 0.2f;
    private int _attackCombo = 0;

    private float _counterTiming = 0.3f;
    private float _counterTimingElapsed = 0f;
    private int _enemyMaxCombo = 3;
    private int _enemyCombo = 0;

    private bool _cameraZoomed = false;

    private float _cameraTargetSize;

    [SerializeField] private Transform cameraZoomFocusing;
    [SerializeField] private Transform cameraNormalFocusing;
    
    [SerializeField] private float cameraZoomSpeed;
    [SerializeField] private float cameraZoomSize;
    [SerializeField] private float cameraNormalSize;

    [SerializeField] private Camera mainCamera;

    [SerializeField] private GameObject timingBarObject;
    [SerializeField] private RectTransform timingBar;
    [SerializeField] private RectTransform goodTimingRange;
    [SerializeField] private float timingHandleMinX, timingHandleMaxX;

    [SerializeField] private GameObject counterUIObject;
    [SerializeField] private Image counterTiming;

    [SerializeField] private Animator gladiatorAnimator;
    [SerializeField] private Animator enemyAnimator;

    // 0 : attack, 1 : normal, 2 : dodge
    [SerializeField] private Transform[] gladiatorPositions;
    [SerializeField] private Transform[] enemyPositions;

    private InputAction _attackAction;
    private void Start()
    {
        cameraNormalSize = mainCamera.orthographicSize;
        
        _attackAction  = InputSystem.actions.FindAction("Attack");
        _attackAction.started += AttackStart;

        _isAttackAvailable = true;
    }

    private void Update()
    {
        // Camera zooming management
        if (_cameraZoomed)
        {
            CameraZoomIn();
        }
        else
        {
            CameraZoomOut();
        }

        if (_isAttacking) 
            AttackTimingUpdate();

        if (_isCountering)
            CounterTimingUpdate();
        
        if (_attackCooldownElapsed > 0)
        {
            _attackCooldownElapsed -= Time.deltaTime;

            if (_attackCooldownElapsed > attackCooldown * 0.5f && _attackCooldownElapsed < attackCooldown * 0.6f)
            {
                if (!_isCountering) StartEnemyAttack();
            }
        }
    }

    private void AttackStart(InputAction.CallbackContext ctx)
    {
        if (_isAttacking || _isCountering) return;
        
        _timingBarElapsed = 0f;
        _attackCombo = 0;
        
        _isAttacking = true;
        _timingBar = 0.7f;
        
        _attackAction.started -= AttackStart;
        _attackAction.started += AttackTimingCheck;

        _cameraZoomed = true;

        TurnAttackUI(true);
    }

    private void AttackTimingUpdate()
    {
        if (!_isAttackAvailable) return;
        
        _timingBarElapsed += Time.deltaTime;

        if (_timingBarElapsed >= _timingBar)
            ComebackFromAttack();

        timingBar.anchoredPosition 
            = Vector2.right * (_timingBarElapsed / _timingBar) * (timingHandleMaxX - timingHandleMinX) + Vector2.right * timingHandleMinX;
        
        _attackCooldownElapsed = attackCooldown;
    }

    private void AttackTimingCheck(InputAction.CallbackContext ctx)
    {
        if (!_isAttacking || !_isAttackAvailable) return;
        
        var range = _timingBar * _goodTimingRange;
        
        var min = _timingBar * _goodTiming - range / 2;
        var max = _timingBar * _goodTiming + range / 2;

        if (_timingBarElapsed < min || _timingBarElapsed > max)
        {
            gladiatorAnimator.Play("gladiator_idle");
            ComebackFromAttack();
            return;
        }
        else
        {
            Debug.Log("GOOD!");
            enemyAnimator.Play("enemy_hit");
            StartCoroutine(ShakeCamera());
        }
        
        int a = Random.Range(0, 2) + 1;
        string name = "gladiator_attack" + a;
        gladiatorAnimator.Play(name);

        TurnAttackUI(false);

        if (_attackCombo < 4)
        {
            _attackCombo += 1;
            _timingBarElapsed = 0f;

            _timingBar *= 0.75f;
            
            StartCoroutine(StartNextAttackCombo());
        }
        else
        {
            ComebackFromAttack();
        }
    }

    private void TurnAttackUI(bool onoff)
    {
        _isAttackAvailable = onoff;
        timingBarObject.SetActive(onoff);

        if (onoff)
        {
            int a = Random.Range(0, 2) + 1;
            string name = "gladiator_attack" + a + "_ready";
            gladiatorAnimator.Play(name);
        }

        Time.timeScale = onoff ? 0.5f : 1f;
    }
    
    private IEnumerator StartNextAttackCombo()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        
        TurnAttackUI(true);
    }
    
    private void ComebackFromAttack()
    {
        _isAttacking = false;
        _attackAction.started += AttackStart;
        _attackAction.started -= AttackTimingCheck;
        
        _cameraZoomed = false;

        _attackCooldownElapsed = attackCooldown;
        
        timingBarObject.SetActive(false);
    }

    private void StartEnemyAttack()
    {
        if (_isAttacking) return;
        
        _isCountering = true;
        _isAttackAvailable = true;
        
        _attackAction.started -= AttackStart;
        _attackAction.started += HitCounter;

        _enemyCombo = 0;
        _counterTiming = 0.3f;
        _counterTimingElapsed = 0;

        _cameraZoomed = true;
        
        TurnCounterUI(true);
    }

    private void CounterTimingUpdate()
    {
        if (!_isAttackAvailable) return;
        _counterTimingElapsed += Time.deltaTime;
        
        counterTiming.fillAmount = _counterTimingElapsed / _counterTiming;
        
        if (_counterTimingElapsed >= _counterTiming)
        {
            enemyAnimator.Play("enemy_attack1");
            gladiatorAnimator.Play("gladiator_hit");

            StartCoroutine(ShakeCamera());
            
            TurnCounterUI(false);
            ComebackFromCountering();
        }
    }
    
    private void TurnCounterUI(bool onoff)
    {
        _isAttackAvailable = onoff;
        counterUIObject.SetActive(onoff);
        
        if (onoff)
        {
            int a = Random.Range(0, 2) + 1;
            string name = "enemy_attack" + a + "_ready";
            enemyAnimator.Play(name);
        }
        
        Time.timeScale = onoff ? 0.5f : 1f;
    }

    private void HitCounter(InputAction.CallbackContext ctx)
    {
        if (!_isAttackAvailable)
        {
            gladiatorAnimator.Play("gladiator_attack2");
            _counterTiming = 0.01f;
            return;
        }
        TurnCounterUI(false);

        _counterTimingElapsed = 0;
        
        enemyAnimator.Play("enemy_attack1");
        gladiatorAnimator.Play("gladiator_attack2");
        
        StartCoroutine(ShakeCamera());

        if (_enemyCombo < _enemyMaxCombo)
        {
            _enemyCombo += 1;
            _counterTiming *= 0.75f;
            StartCoroutine(StartNextCounter());
        }
        else
        {
            ComebackFromCountering();
        }
    }

    private IEnumerator StartNextCounter()
    {
        yield return new WaitForSecondsRealtime(0.5f + Random.Range(-0.2f, 0.2f));
        TurnCounterUI(true);
    }

    private void ComebackFromCountering()
    {
        _isCountering = false;
        
        _attackAction.started += AttackStart;
        _attackAction.started -= HitCounter;
        
        _cameraZoomed = false;
    }
    

    private IEnumerator ShakeCamera()
    {
        mainCamera.transform.position = new Vector3(
            mainCamera.transform.position.x + Random.Range(-1f, 1f), 
            mainCamera.transform.position.y  + Random.Range(-1f, 1f),
            mainCamera.transform.position.z);
        
        yield return new WaitForSecondsRealtime(0.1f);
        
        mainCamera.transform.position = new Vector3(
            mainCamera.transform.position.x + Random.Range(-1f, 1f), 
            mainCamera.transform.position.y  + Random.Range(-1f, 1f),
            mainCamera.transform.position.z);
    }
    
    private void CameraZoomIn()
    {
        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, cameraZoomSize, cameraZoomSpeed * Time.deltaTime);
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraZoomFocusing.position, cameraZoomSpeed * Time.deltaTime);
    }
    
    private void CameraZoomOut()
    {
        mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, cameraNormalSize, cameraZoomSpeed * Time.deltaTime);
        mainCamera.transform.position = Vector3.Lerp(mainCamera.transform.position, cameraNormalFocusing.position, cameraZoomSpeed * Time.deltaTime);
    }
}
