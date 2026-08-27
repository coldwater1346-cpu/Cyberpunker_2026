using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float _bulletSpeed = 40f;
    [SerializeField] private float _lifeTime = 3f;

    private float _damage = 10f;
    public float Damage { get { return _damage; } }

    private bool _isFired = false;
    private float _lifeTimer = 0f;

    public void Init(float damage = 10f)
    {
        _damage = damage;
    }

    private void Update()
    {
        if (!_isFired) return;

        float scale = 1f;

        if (TimeControlManager.Instance != null)
            scale = TimeControlManager.Instance.WorldScale;

        transform.position += transform.forward * _bulletSpeed * scale * Time.deltaTime;

        _lifeTimer += Time.deltaTime;

        if (_lifeTimer >= _lifeTime)
        {
            ReturnBullet();
        }
    }

    public void Fire()
    {
        _isFired = true;
        _lifeTimer = 0f;
    }

    private void OnTriggerEnter(Collider other)
    {
     

        ReturnBullet();
    }

    private void ReturnBullet()
    {
        _isFired = false;
        _lifeTimer = 0f;

        if (PoolManager.Instance != null)
        {
            PoolManager.Instance.Return(gameObject, PoolKey.BULLET);
        }
    }

    private void OnDisable()
    {
        _isFired = false;
        _lifeTimer = 0f;
    }
}