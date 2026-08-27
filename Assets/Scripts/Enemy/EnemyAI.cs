using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyState
{
    IDLE,
    PATROL,
    SELFCHASE,
    ALERTCHASE,
    COMBAT,
    DAMAGED,
    DEAD
}

public class EnemyAI : MonoBehaviour
{
    [Header("Rotate Offset")]
    [SerializeField] private float _yawOffset = -30f;

    [Header("State")]
    [SerializeField] private EnemyState _state = EnemyState.IDLE;

    private Animator _anim;
    private Collider _collider;
    private NavMeshAgent _agent;
    private EnemyAttack _enemyAttack;

    private Coroutine _currentRoutine;

    [Header("Target")]
    [SerializeField] private GameObject _target;

    [Header("Idle State")]
    [SerializeField] private float _idleMinTime = 2f;
    [SerializeField] private float _idleMaxTime = 4f;
    private float _idleTime = 0f;
    private float _elapsedTime = 0f;

    [Header("Patrol State")]
    [SerializeField] private Transform _patrolPointA;
    [SerializeField] private Transform _patrolPointB;
    [SerializeField] private float _patrolMoveSpeed = 3f;
    [SerializeField] private float _patrolAccel = 5f;
    [SerializeField] private float _patrolAngularSpeed = 120f;

    private Transform _currentPatrolPoint;

    [Header("Chase State")]
    [SerializeField] private float _chaseSpeed = 5f;
    [SerializeField] private float _chaseAccel = 10f;
    [SerializeField] private float _chaseAngularSpeed = 180f;
    [SerializeField] private float _chaseRange = 15f;

    [Header("Combat State")]
    [SerializeField] private float _combatRange = 10f;
    [SerializeField] private float _keepDistance = 7f;
    [SerializeField] private float _combatMoveSpeed = 3.5f;
    [SerializeField] private float _combatAccel = 8f;
    [SerializeField] private float _combatAngularSpeed = 180f;
    [SerializeField] private float _attackInterval = 1.5f;
    [SerializeField] private float _attackDelay = 0.2f;
    [SerializeField] private float _faceRotateSpeed = 10f;

    [Header("Hp")]
    [SerializeField] private float _hp = 20f;
    private bool _isAlive = true;
    public bool IsAlive { get { return _isAlive; } }

    [Header("Damaged State")]
    [SerializeField] private float _damageStunTime = 0.5f;

 

    // Animation Parameter Hash
    private int _hashPatrol = Animator.StringToHash("Patrol");
    private int _hashChase = Animator.StringToHash("Chase");
    private int _hashCombat = Animator.StringToHash("Combat");
    private int _hashDamaged = Animator.StringToHash("Damaged");
    private int _hashDead = Animator.StringToHash("Dead");
    private int _hashMove = Animator.StringToHash("IsMove");

    private void Start()
    {
        _anim = GetComponent<Animator>();
        _collider = GetComponent<Collider>();
        _agent = GetComponent<NavMeshAgent>();
        _enemyAttack = GetComponent<EnemyAttack>();

        if (_target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                _target = playerObj;
            }
        }

        _currentPatrolPoint = _patrolPointA;

        ChangeState(EnemyState.IDLE);
    }

    private void Update()
    {
        if (!_isAlive)
        {
            if (_state != EnemyState.DEAD)
            {
                ChangeState(EnemyState.DEAD);
            }

            return;
        }

        UpdateAgentSpeedByWorldScale();
        UpdateMoveAnimation();
    }

    private float GetWorldScale()
    {
        if (TimeControlManager.Instance == null)
            return 1f;

        return TimeControlManager.Instance.WorldScale;
    }

    private void UpdateAgentSpeedByWorldScale()
    {
        if (!CanUseAgent())
            return;

        float scale = GetWorldScale();

        switch (_state)
        {
            case EnemyState.PATROL:
                _agent.speed = _patrolMoveSpeed * scale;
                _agent.acceleration = _patrolAccel;
                _agent.angularSpeed = _patrolAngularSpeed;
                break;

            case EnemyState.SELFCHASE:
            case EnemyState.ALERTCHASE:
                _agent.speed = _chaseSpeed * scale;
                _agent.acceleration = _chaseAccel;
                _agent.angularSpeed = _chaseAngularSpeed;
                break;

            case EnemyState.COMBAT:
                _agent.speed = _combatMoveSpeed * scale;
                _agent.acceleration = _combatAccel;
                _agent.angularSpeed = _combatAngularSpeed;
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_isAlive) return;

        if (other.CompareTag("Bullet"))
        {
            Debug.Log("적이 총알에 맞음");

            BulletController bullet = other.GetComponent<BulletController>();
            if (bullet != null)
            {
                TakeDamage(bullet.Damage);
            }
        }
    }

    private void ChangeState(EnemyState newState)
    {
        if (!_isAlive && newState != EnemyState.DEAD)
            return;

        if (_currentRoutine != null && _state == newState && newState != EnemyState.DAMAGED)
            return;

        if (_currentRoutine != null)
        {
            StopCoroutine(_currentRoutine);
            _currentRoutine = null;
        }

        _state = newState;
        ApplyAgentSetting(_state);

        switch (_state)
        {
            case EnemyState.IDLE:
                _currentRoutine = StartCoroutine(IdleRoutine());
                break;

            case EnemyState.PATROL:
                _currentRoutine = StartCoroutine(PatrolRoutine());
                break;

            case EnemyState.SELFCHASE:
                _currentRoutine = StartCoroutine(SelfChaseRoutine());
                break;

            case EnemyState.ALERTCHASE:
                _currentRoutine = StartCoroutine(AlertChaseRoutine());
                break;

            case EnemyState.COMBAT:
                _currentRoutine = StartCoroutine(CombatRoutine());
                break;

            case EnemyState.DAMAGED:
                _currentRoutine = StartCoroutine(DamagedRoutine());
                break;

            case EnemyState.DEAD:
                _currentRoutine = StartCoroutine(DeadRoutine());
                break;
        }
    }

    private IEnumerator IdleRoutine()
    {
        if (!_isAlive) yield break;

        SetAnimation();

        _idleTime = Random.Range(_idleMinTime, _idleMaxTime);
        _elapsedTime = 0f;

        while (true)
        {
            if (!_isAlive) yield break;

            yield return null;

            _elapsedTime += Time.deltaTime;
            if (_elapsedTime >= _idleTime)
            {
                ChangeState(EnemyState.PATROL);
                yield break;
            }
        }
    }

    private IEnumerator PatrolRoutine()
    {
        if (!_isAlive) yield break;

        SetAnimation(hashPatrol: true);

        if (_currentPatrolPoint == null)
        {
            ChangeState(EnemyState.IDLE);
            yield break;
        }

        SafeSetStopped(false);
        SafeSetDestination(_currentPatrolPoint.position);

        while (true)
        {
            if (!_isAlive) yield break;

            if (CanUseAgent())
            {
                if (!_agent.pathPending &&
                    _agent.remainingDistance <= Mathf.Max(_agent.stoppingDistance, 0.1f))
                {
                    ChangePatrolPoint();
                    ChangeState(EnemyState.IDLE);
                    yield break;
                }
            }

            yield return null;
        }
    }

    private IEnumerator SelfChaseRoutine()
    {
        if (!_isAlive) yield break;

        SetAnimation(hashChase: true);

        while (true)
        {
            if (!_isAlive) yield break;

            yield return null;

            if (_target == null)
            {
                ChangeState(EnemyState.IDLE);
                yield break;
            }

            float dist = Vector3.Distance(transform.position, _target.transform.position);

            if (dist <= _combatRange)
            {
                ChangeState(EnemyState.COMBAT);
                yield break;
            }

            if (dist > _chaseRange)
            {
                ChangeState(EnemyState.PATROL);
                yield break;
            }

            SafeSetStopped(false);
            SafeSetDestination(_target.transform.position);
        }
    }

    private IEnumerator AlertChaseRoutine()
    {
        if (!_isAlive) yield break;

        SetAnimation(hashChase: true);

        while (true)
        {
            if (!_isAlive) yield break;

            yield return null;

            if (_target == null)
            {
                ChangeState(EnemyState.IDLE);
                yield break;
            }

            float dist = Vector3.Distance(transform.position, _target.transform.position);

            if (dist <= _combatRange)
            {
                ChangeState(EnemyState.COMBAT);
                yield break;
            }

            SafeSetStopped(false);
            SafeSetDestination(_target.transform.position);
        }
    }

    private IEnumerator CombatRoutine()
    {
        if (!_isAlive) yield break;

        SetAnimation(hashCombat: true);

        float attackTimer = 0f;

        while (true)
        {
            if (!_isAlive) yield break;

            yield return null;

            if (_target == null)
            {
                ChangeState(EnemyState.IDLE);
                yield break;
            }

            float dist = Vector3.Distance(transform.position, _target.transform.position);

            if (dist > _combatRange)
            {
                if (dist <= _chaseRange)
                {
                    ChangeState(EnemyState.SELFCHASE);
                }
                else
                {
                    ChangeState(EnemyState.IDLE);
                }
                yield break;
            }

            if (CanUseAgent())
            {
                if (dist > _keepDistance)
                {
                    SafeSetStopped(false);
                    SafeSetDestination(_target.transform.position);
                }
                else
                {
                    SafeSetStopped(true);
                    SafeResetPath();
                    FaceTarget();
                }
            }
            else
            {
                FaceTarget();
            }

            attackTimer += Time.deltaTime;

            if (attackTimer >= _attackInterval)
            {
                if (dist <= _combatRange)
                {
                    yield return new WaitForSeconds(_attackDelay);

                    if (!_isAlive) yield break;

                    if (_enemyAttack != null && _target != null)
                    {
                        _enemyAttack.Fire(_target);
                    }
                }

                attackTimer = 0f;
            }
        }
    }

    private IEnumerator DamagedRoutine()
    {
        if (!_isAlive) yield break;

        Debug.Log("DamagedRoutine 시작");

        if (_anim != null)
        {
            Debug.Log("Damaged Trigger 실행");
            _anim.SetTrigger(_hashDamaged);
        }

        SafeSetStopped(true);
        SafeResetPath();

        yield return new WaitForSeconds(_damageStunTime);

        if (!_isAlive) yield break;

        ChangeState(EnemyState.IDLE);
    }

    private IEnumerator DeadRoutine()
    {
        SetAnimation();

        SafeSetStopped(true);
        SafeResetPath();

        if (_anim != null)
        {
            Debug.Log("Dead Trigger 실행");
            _anim.SetTrigger(_hashDead);
        }

        if (_collider != null)
        {
            _collider.enabled = false;
        }

        yield return null;

        if (_agent != null && _agent.enabled)
        {
            _agent.enabled = false;
        }

        yield return null;
    }

    private void FaceTarget()
    {
        if (_target == null) return;

        Vector3 dir = _target.transform.position - transform.position;
        dir.y = 0f;

        if (dir == Vector3.zero) return;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        targetRot *= Quaternion.Euler(0f, _yawOffset, 0f);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            _faceRotateSpeed * Time.deltaTime
        );
    }

    private void UpdateMoveAnimation()
    {
        if (_anim == null) return;

        bool isMove = false;

        if (CanUseAgent() && !_agent.isStopped)
        {
            isMove = _agent.velocity.magnitude > 0.1f;
        }

        _anim.SetBool(_hashMove, isMove);
    }

    public void TakeDamage(float damage)
    {
        if (!_isAlive) return;

        _hp -= damage;

        if (_hp <= 0f)
        {
            Die();
            return;
        }

        ChangeState(EnemyState.DAMAGED);
    }

    private void Die()
    {
        if (!_isAlive) return;

        _isAlive = false;
        _hp = 0f;

        if (_currentRoutine != null)
        {
            StopCoroutine(_currentRoutine);
            _currentRoutine = null;
        }

        SafeSetStopped(true);
        SafeResetPath();

        EnemyUnit unit = GetComponent<EnemyUnit>();
        if (unit != null)
        {
            unit.DropCardKey();
        }

        ChangeState(EnemyState.DEAD);
    }

    private void ChangePatrolPoint()
    {
        if (_currentPatrolPoint == _patrolPointA)
        {
            _currentPatrolPoint = _patrolPointB;
        }
        else
        {
            _currentPatrolPoint = _patrolPointA;
        }
    }

    public void StartSelfChase(GameObject target)
    {
        if (!_isAlive) return;
        if (target == null) return;
        if (_state == EnemyState.DEAD) return;
        if (_state == EnemyState.COMBAT) return;

        _target = target;

        if (_state == EnemyState.SELFCHASE)
            return;

        ChangeState(EnemyState.SELFCHASE);
    }

    public void StartAlertChase(GameObject target)
    {
        if (!_isAlive) return;
        if (target == null) return;
        if (_state == EnemyState.DEAD) return;
        if (_state == EnemyState.COMBAT) return;

        _target = target;

        if (_state == EnemyState.ALERTCHASE)
            return;

        ChangeState(EnemyState.ALERTCHASE);
    }

    public void SetAnimation(bool hashPatrol = false, bool hashChase = false, bool hashCombat = false)
    {
        if (_anim == null) return;

        _anim.SetBool(_hashPatrol, hashPatrol);
        _anim.SetBool(_hashChase, hashChase);
        _anim.SetBool(_hashCombat, hashCombat);
    }

    private void ApplyAgentSetting(EnemyState state)
    {
        if (!CanUseAgent())
            return;

        float scale = GetWorldScale();

        switch (state)
        {
            case EnemyState.IDLE:
                SafeSetVelocityZero();
                _agent.speed = 0f;
                _agent.acceleration = 0f;
                _agent.angularSpeed = 0f;
                SafeSetStopped(true);
                SafeResetPath();
                _agent.stoppingDistance = 0f;
                break;

            case EnemyState.PATROL:
                _agent.speed = _patrolMoveSpeed * scale;
                _agent.acceleration = _patrolAccel;
                _agent.angularSpeed = _patrolAngularSpeed;
                SafeSetStopped(false);
                _agent.stoppingDistance = 0f;
                break;

            case EnemyState.SELFCHASE:
                _agent.speed = _chaseSpeed * scale;
                _agent.acceleration = _chaseAccel;
                _agent.angularSpeed = _chaseAngularSpeed;
                SafeSetStopped(false);
                _agent.stoppingDistance = _keepDistance;
                break;

            case EnemyState.ALERTCHASE:
                _agent.speed = _chaseSpeed * scale;
                _agent.acceleration = _chaseAccel;
                _agent.angularSpeed = _chaseAngularSpeed;
                SafeSetStopped(false);
                _agent.stoppingDistance = _keepDistance;
                break;

            case EnemyState.COMBAT:
                _agent.speed = _combatMoveSpeed * scale;
                _agent.acceleration = _combatAccel;
                _agent.angularSpeed = _combatAngularSpeed;
                SafeSetStopped(false);
                _agent.stoppingDistance = _keepDistance;
                break;

            case EnemyState.DAMAGED:
                SafeSetVelocityZero();
                _agent.speed = 0f;
                _agent.acceleration = 0f;
                _agent.angularSpeed = 0f;
                SafeSetStopped(true);
                SafeResetPath();
                _agent.stoppingDistance = 0f;
                break;

            case EnemyState.DEAD:
                SafeSetVelocityZero();
                _agent.speed = 0f;
                _agent.acceleration = 0f;
                _agent.angularSpeed = 0f;
                SafeSetStopped(true);
                SafeResetPath();
                _agent.stoppingDistance = 0f;
                break;
        }
    }

    private bool CanUseAgent()
    {
        if (_agent == null) return false;
        if (!_agent.enabled) return false;
        if (!_agent.isActiveAndEnabled) return false;
        if (!_agent.isOnNavMesh) return false;

        return true;
    }

    private void SafeSetStopped(bool value)
    {
        if (!CanUseAgent()) return;
        _agent.isStopped = value;
    }

    private void SafeResetPath()
    {
        if (!CanUseAgent()) return;
        _agent.ResetPath();
    }

    private void SafeSetDestination(Vector3 destination)
    {
        if (!CanUseAgent()) return;
        _agent.SetDestination(destination);
    }

    private void SafeSetVelocityZero()
    {
        if (!CanUseAgent()) return;
        _agent.velocity = Vector3.zero;
    }
}