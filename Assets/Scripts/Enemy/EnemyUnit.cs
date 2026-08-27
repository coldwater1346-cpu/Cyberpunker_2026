using UnityEngine;
using System.Collections;

public class EnemyUnit : MonoBehaviour
{
    [Header("Enemy")]
    [SerializeField] private Animator _anim;
    [SerializeField] private GameObject _backZone;
    [SerializeField] private EnemyAI _enemyAI;

    [Header("Card Key Drop")]
    [SerializeField] private GameObject _cardKeyPrefab;
    [SerializeField] private Transform _dropPoint;
    [SerializeField] private float _dropDelay = 1.0f;

    private bool _isDead = false;
    private bool _isDropped = false;

    public bool IsDead => _isDead;

    private void Awake()
    {
        if (_enemyAI == null)
            _enemyAI = GetComponent<EnemyAI>();
    }

    public void Die()
    {
        if (_isDead) return;

        _isDead = true;

        if (_backZone != null)
            _backZone.SetActive(false);

        if (_enemyAI != null)
            _enemyAI.TakeDamage(9999f);

        StartCoroutine(CoDropCardKey());
    }

    //  EnemyAI에서도 호출할 함수
    public void DropCardKey()
    {
        if (_isDropped) return;

        if (_backZone != null)
            _backZone.SetActive(false);

        StartCoroutine(CoDropCardKey());
    }

    private IEnumerator CoDropCardKey()
    {
        if (_isDropped) yield break;

        yield return new WaitForSeconds(_dropDelay);

        if (_cardKeyPrefab != null)
        {
            Transform point = _dropPoint != null ? _dropPoint : transform;

            Instantiate(
                _cardKeyPrefab,
                point.position,
                point.rotation
            );

            _isDropped = true;
        }
    }
}