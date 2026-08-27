using UnityEngine;
using System.Collections;

public class BreakableSecretWall : MonoBehaviour
{
    [Header("Hit Settings")]
    [SerializeField] private int _maxHitCount = 10;
    [SerializeField] private string _bulletTag = "Bullet";

    [Header("Collider Settings")]
    [SerializeField] private Collider _solidCollider;
    [SerializeField] private Collider _hitTriggerCollider;

    [Header("Split Spawn")]
    [SerializeField] private Transform _splitSpawnPoint;
    [SerializeField] private Vector3 _splitSpawnRange = new Vector3(1f, 1f, 1f);

    [Header("Blink Settings")]
    [SerializeField] private Color _blinkColor = Color.red;
    [SerializeField] private float _blinkDuration = 0.08f;
    [SerializeField] private int _blinkCount = 2;

    [Header("Split Settings")]
    [SerializeField] private int _splitCount = 30;
    [SerializeField] private PrimitiveType _primitiveType = PrimitiveType.Cube;
    [SerializeField] private float _splitScale = 0.25f;
    [SerializeField] private float _splitForce = 3f;
    [SerializeField] private float _splitTorque = 5f;
    [SerializeField] private float _splitPieceLifeTime = 2f;

    [Header("Split Material")]
    [SerializeField] private Material _splitMaterial;

    private int _currentHitCount = 0;
    private bool _isBroken = false;
    private bool _isBlinking = false;

    private MeshRenderer _meshRenderer;
    private Color _originalColor = Color.white;

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();

        if (_meshRenderer != null && _meshRenderer.material.HasProperty("_Color"))
            _originalColor = _meshRenderer.material.color;

        if (_solidCollider == null)
            _solidCollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_isBroken) return;
        if (!other.CompareTag(_bulletTag)) return;

        _currentHitCount++;

        Debug.Log($"비밀 벽 피격 횟수 : {_currentHitCount} / {_maxHitCount}");

        other.gameObject.SetActive(false);

        if (!_isBlinking)
            StartCoroutine(BlinkRed());

        if (_currentHitCount >= _maxHitCount)
            StartCoroutine(BreakWall());
    }

    private IEnumerator BlinkRed()
    {
        if (_meshRenderer == null)
            yield break;

        _isBlinking = true;

        for (int i = 0; i < _blinkCount; i++)
        {
            if (_meshRenderer.material.HasProperty("_Color"))
                _meshRenderer.material.color = _blinkColor;

            yield return new WaitForSeconds(_blinkDuration);

            if (_meshRenderer.material.HasProperty("_Color"))
                _meshRenderer.material.color = _originalColor;

            yield return new WaitForSeconds(_blinkDuration);
        }

        if (_meshRenderer.material.HasProperty("_Color"))
            _meshRenderer.material.color = _originalColor;

        _isBlinking = false;
    }

    private IEnumerator BreakWall()
    {
        _isBroken = true;

        if (_solidCollider != null)
            _solidCollider.enabled = false;

        if (_hitTriggerCollider != null)
            _hitTriggerCollider.enabled = false;

        if (_meshRenderer != null)
            _meshRenderer.enabled = false;

        Vector3 centerPos = transform.position;

        if (_splitSpawnPoint != null)
            centerPos = _splitSpawnPoint.position;

        for (int i = 0; i < _splitCount; i++)
        {
            GameObject piece = GameObject.CreatePrimitive(_primitiveType);

            Vector3 randomOffset = new Vector3(
                Random.Range(-_splitSpawnRange.x * 0.5f, _splitSpawnRange.x * 0.5f),
                Random.Range(-_splitSpawnRange.y * 0.5f, _splitSpawnRange.y * 0.5f),
                Random.Range(-_splitSpawnRange.z * 0.5f, _splitSpawnRange.z * 0.5f)
            );

            piece.transform.position = centerPos + randomOffset;
            piece.transform.rotation = Random.rotation;
            piece.transform.localScale = Vector3.one * _splitScale;

            MeshRenderer pieceRenderer = piece.GetComponent<MeshRenderer>();
            if (pieceRenderer != null)
            {
                if (_splitMaterial != null)
                {
                    pieceRenderer.material = _splitMaterial;
                }

                if (pieceRenderer.material.HasProperty("_Color"))
                {
                    pieceRenderer.material.color = _originalColor;
                }
            }

            Rigidbody rb = piece.AddComponent<Rigidbody>();

            Vector3 randomDir = Random.onUnitSphere;
            randomDir.y = Mathf.Abs(randomDir.y) + 0.3f;

            rb.AddForce(randomDir * _splitForce, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * _splitTorque, ForceMode.Impulse);

            Destroy(piece, _splitPieceLifeTime);
        }

        yield return null;

        Destroy(gameObject);
    }
}