using UnityEngine;
using System.Collections;

public class PlayerAttack : MonoBehaviour
{
    [Header("Fire")]
    [SerializeField] private Transform _firePosition;
    [SerializeField] private float _bulletDamage = 10f;
    [SerializeField] private float _spawnOffset = 0.3f;
    [SerializeField] private float _fireInterval = 0.2f;

    [Header("BackAttack")]
    [SerializeField] private Animator _anim;
    [SerializeField] private KeyCode _attackKey = KeyCode.F;
    [SerializeField] private float _killDelay = 0.35f;

    private EnemyUnit _targetEnemy;
    private bool _isAction = false;
    private float _fireCooldown = 0f;

    private PlayerMovement _playerMovement;
    private PlayerHealth _playerHealth;

    private void Awake()
    {
        if (_anim == null)
            _anim = GetComponentInChildren<Animator>();

        _playerMovement = GetComponent<PlayerMovement>();
        _playerHealth = GetComponent<PlayerHealth>();
    }

    private void Update()
    {
        if (_playerHealth != null && _playerHealth.IsDead)
        {
            _isAction = false;
            return;
        }

        if (_fireCooldown > 0f)
        {
            _fireCooldown -= Time.deltaTime;
        }

        if (_isAction) return;

        if (_playerMovement != null &&
            _playerMovement.IsPistol &&
            Input.GetButtonDown("Fire1") &&
            _fireCooldown <= 0f)
        {
            FireBullet();
            _fireCooldown = _fireInterval;
        }

        if (Input.GetKeyDown(_attackKey))
        {
            TryBackAttack();
        }
    }

    private void FireBullet()
    {
        if (_playerHealth != null && _playerHealth.IsDead) return;
        if (_firePosition == null) return;
        if (PoolManager.Instance == null) return;

        GameObject bullet = PoolManager.Instance.Get(PoolKey.BULLET);
        if (bullet == null) return;

        bullet.transform.position = _firePosition.position + _firePosition.forward * _spawnOffset;
        bullet.transform.rotation = _firePosition.rotation;

        BulletController bulletCtrl = bullet.GetComponent<BulletController>();
        if (bulletCtrl != null)
        {
            bulletCtrl.Init(_bulletDamage);
            bulletCtrl.Fire();
        }

        GameObject fireEffect = PoolManager.Instance.Get(PoolKey.EFFECT);
        if (fireEffect != null)
        {
            PooledParticle particle = fireEffect.GetComponent<PooledParticle>();
            if (particle != null)
            {
                particle.RestartAt(_firePosition);
            }
        }
    }

    public void SetBackStabTarget(EnemyUnit enemy)
    {
        if (enemy == null || enemy.IsDead) return;
        _targetEnemy = enemy;
    }

    public void ClearBackStabTarget(EnemyUnit enemy)
    {
        if (_targetEnemy == enemy)
            _targetEnemy = null;
    }

    private void TryBackAttack()
    {
        if (_playerHealth != null && _playerHealth.IsDead) return;
        if (_targetEnemy == null) return;
        if (_targetEnemy.IsDead) return;
        if (_isAction) return;

        StartCoroutine(CoBackAttack());
    }

    private IEnumerator CoBackAttack()
    {
        _isAction = true;

        if (_playerHealth != null && _playerHealth.IsDead)
        {
            _isAction = false;
            yield break;
        }

        if (_anim == null)
        {
            Debug.LogError("BackAttack 실패 : Animator 참조가 없습니다.");
            _isAction = false;
            yield break;
        }

        if (_anim.runtimeAnimatorController == null)
        {
            Debug.LogError($"BackAttack 실패 : Animator Controller가 없습니다. 대상 오브젝트 = {_anim.gameObject.name}");
            _isAction = false;
            yield break;
        }

        _anim.ResetTrigger("BackAttack");
        _anim.SetTrigger("BackAttack");

        yield return new WaitForSeconds(_killDelay);

        if (_playerHealth != null && _playerHealth.IsDead)
        {
            _isAction = false;
            yield break;
        }

        if (_targetEnemy != null && !_targetEnemy.IsDead)
            _targetEnemy.Die();

        _isAction = false;
    }
}