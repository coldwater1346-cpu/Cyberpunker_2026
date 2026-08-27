using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("HP")]
    [SerializeField] private float _maxHp = 100f;
    private float _currentHp;

    [Header("Death")]
    [SerializeField] private Animator _anim;
    [SerializeField] private float _respawnDelay = 3f;

    [Header("Respawn")]
    [SerializeField] private PlayerRespawn _playerRespawn;

    [Header("Hit Effect")]
    [SerializeField] private Renderer[] _renderers;
    [SerializeField] private Color _hitColor = Color.red;
    [SerializeField] private float _flashInterval = 0.1f;
    [SerializeField] private int _flashCount = 2;

    private bool _isDead = false;
    public bool IsDead
    {
        get { return _isDead; }
    }

    private Material[] _mats;
    private Color[] _originColors;
    private Coroutine _hitEffectRoutine;

    private void Start()
    {
        _currentHp = _maxHp;

        if (_anim == null)
            _anim = GetComponent<Animator>();

        if (_playerRespawn == null)
            _playerRespawn = GetComponent<PlayerRespawn>();

        if (_renderers != null && _renderers.Length > 0)
        {
            _mats = new Material[_renderers.Length];
            _originColors = new Color[_renderers.Length];

            for (int i = 0; i < _renderers.Length; i++)
            {
                if (_renderers[i] == null) continue;

                _mats[i] = _renderers[i].material;

                if (_mats[i].HasProperty("_BaseColor"))
                    _originColors[i] = _mats[i].GetColor("_BaseColor");
                else
                    _originColors[i] = _mats[i].color;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) return;

        _currentHp -= damage;

        if (_hitEffectRoutine != null)
            StopCoroutine(_hitEffectRoutine);

        _hitEffectRoutine = StartCoroutine(HitEffectRoutine());

        if (_currentHp <= 0f)
        {
            _currentHp = 0f;
            Die();
        }
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (_anim != null)
            _anim.SetBool("IsDead", true);

        StartCoroutine(RespawnRoutine());
    }

    private IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(_respawnDelay);

        if (_playerRespawn != null)
            _playerRespawn.Respawn();

        _currentHp = _maxHp;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (_anim != null)
            _anim.SetBool("IsDead", false);

        SetAllMaterialColorsToOrigin();

        _isDead = false;
    }

    private IEnumerator HitEffectRoutine()
    {
        if (_mats == null || _mats.Length == 0) yield break;

        for (int i = 0; i < _flashCount; i++)
        {
            SetAllMaterialColors(_hitColor);
            yield return new WaitForSeconds(_flashInterval);

            SetAllMaterialColorsToOrigin();
            yield return new WaitForSeconds(_flashInterval);
        }

        SetAllMaterialColorsToOrigin();
        _hitEffectRoutine = null;
    }

    private void SetAllMaterialColors(Color color)
    {
        if (_mats == null) return;

        for (int i = 0; i < _mats.Length; i++)
        {
            if (_mats[i] == null) continue;

            if (_mats[i].HasProperty("_BaseColor"))
                _mats[i].SetColor("_BaseColor", color);
            else
                _mats[i].color = color;
        }
    }

    private void SetAllMaterialColorsToOrigin()
    {
        if (_mats == null || _originColors == null) return;

        for (int i = 0; i < _mats.Length; i++)
        {
            if (_mats[i] == null) continue;

            if (_mats[i].HasProperty("_BaseColor"))
                _mats[i].SetColor("_BaseColor", _originColors[i]);
            else
                _mats[i].color = _originColors[i];
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isDead) return;

        if (other.CompareTag("Bullet"))
        {
            TakeDamage(10f);
            Destroy(other.gameObject);
        }
    }
}